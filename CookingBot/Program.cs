using EasyOAuth;
using EasyOAuth.Builder;
using EasyOAuth.Extensions;
using EasyTgBot;
using EasyTgBot.Abstract;
using CookingBot;
using CookingBot.Application.Flows.AddRecipe.InContexts;
using CookingBot.Application.Flows.AddRecipe.InContexts.ContextHandlers;
using CookingBot.Application.Flows.EditRecipe.InContext;
using CookingBot.Application.Flows.WantToCook.InContexts;
using CookingBot.Application.Flows.WantToCook.InContexts.ContextHandlers;
using CookingBot.Application.Interfaces;
using CookingBot.Domain.Entity;
using CookingBot.Infrastructure;
using CookingBot.Infrastructure.DataBase;
using CookingBot.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using EditContext = CookingBot.Application.Flows.EditRecipe.EditContext;

var builder = WebApplication.CreateBuilder(args);
var oAuths = OAuths.CreateBuilder(Environment.GetEnvironmentVariable("REDIRECT_URI_OATUTH") ?? throw new Exception("\"REDIRECT_URI_OATUTH\" ENV VAR doesn't exist")); // это редирект когда уже авторизация заверешна и нужно переправить обратно в тг канал.
oAuths.AddOAuth("google", _ =>
    _.SetUriPageAuth("https://accounts.google.com/o/oauth2/v2/auth")
        .SetUriGetAccessToken("https://oauth2.googleapis.com/token")
        .SetResponseType("code")
        .ConfigureApp()
        .SetClientId(Environment.GetEnvironmentVariable("CLIENT_ID") ?? string.Empty)
        .SetClientSecret(Environment.GetEnvironmentVariable("CLIENT_SECRET") ?? string.Empty)
        .SetScope("email")
        .SetRedirectUrl(
            Environment.GetEnvironmentVariable(
                "REDIRECT_URI")) // api... не имеет смысла, так как оно в либе уже статичное
        .SetCustomQuery("grant_type", "authorization_code", QueryFor.GetAccessToken));

builder.Services.AddOAuths<TelegramOAuth, LinkOauthRepository, StrategyToken>(oAuths);


builder.AddLog();

builder.Services.AddControllers().AddNewtonsoftJson();

builder.Services.AddHttpClient();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddOptions<PostgresEntryPointOptions>()
    .Configure(x => x.ConnString = Environment.GetEnvironmentVariable("CONN_STRING"))
    .ValidateDataAnnotations();
// easyTg

builder.Services.AddTelegramCommands();
builder.Services.AddTelegramBotWithController(
    Environment.GetEnvironmentVariable("HOST_FOR_TG") ?? "https://dacfd45e704418.lhr.life",
    Environment.GetEnvironmentVariable("TG_TOKEN") ??
    throw new ArgumentException("NOT HAVE TOKEN FOR BOT TG"));
builder.Services.AddTelegramDbContext<ChatTelegramDb>();


builder.Services.AddScoped<IRecipeRepository, RecipeRepository>();

var registerFlow = new ServiceRegistryFlow();
builder.Services.AddContext<AddingRecipeContext>(x => x
    .AddHandler<AddingName>()
    .AddHandler<AddingIngredients>()
    .AddHandler<AddingInstruction>()
    .AddHandler<SaveRecipe>(), registerFlow);

builder.Services.AddContext<CookContext>(x => x
    .AddHandler<ChoosingDish>()
    .AddHandler<Cooking>(), registerFlow);

builder.Services.AddContext<EditContext>(x =>
        x.AddHandler<ChooseEditRecipe>()
            .AddHandler<SwitchEditItem>(x =>
                x.AddSubHandler<EditInstruction>().AddSubHandler<EditName>().AddSubHandler<EditIngredients>()),
    registerFlow);


builder.Services.AddSingleton<IServiceRegistryFlow>(registerFlow);
var app = builder.Build();

using var scope = app.Services.CreateScope();
using var dbcontex = scope.ServiceProvider.GetService<ChatTelegramDb>();
dbcontex.Database.Migrate();

app.UseMiddleware<ExceptionHandlingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();


app.MapControllers();

app.Run();

public class PostgresEntryPointOptions
{
    public const string Section = "Database";

    public string ConnString { get; set; }
}