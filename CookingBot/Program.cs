using System.Reflection;
using EasyOAuth;
using EasyOAuth.Builder;
using EasyOAuth.Extensions;
using EasyTgBot;
using EasyTgBot.Abstract;
using CookingBot;
using CookingBot.Application.Commands.AddRecipe.Flow;
using CookingBot.Application.Commands.AddRecipe.Flow.ContextHandlers;
using CookingBot.Application.Flows.AddRecipe.Contexts.ContextHandlers;
using CookingBot.Application.Flows.AddRecipe.InContexts;
using CookingBot.Application.Flows.AddRecipe.InContexts.ContextHandlers;
using CookingBot.Application.Flows.WantToCook.InContexts;
using CookingBot.Application.Flows.WantToCook.InContexts.ContextHandlers;
using CookingBot.Application.Interfaces;
using CookingBot.Domain.Entity;
using CookingBot.Infrastructure;
using CookingBot.Infrastructure.DataBase;
using CookingBot.Infrastructure.Repositories;
using CookingBot.Infrastucture.Repositories;
using Vostok.Logging.Abstractions;
using Vostok.Logging.Console;
using Vostok.Logging.Microsoft;

var builder = WebApplication.CreateBuilder(args);
var oAuths = OAuths.CreateBuilder();
oAuths.AddOAuth("google", _ =>
    _.SetUriPageAuth("https://accounts.google.com/o/oauth2/v2/auth")
        .SetUriGetAccessToken("https://oauth2.googleapis.com/token")
        .SetResponseType("code")
        .ConfigureApp()
        .SetClientId(Environment.GetEnvironmentVariable("CLIENT_ID") ?? string.Empty)
        .SetClientSecret(Environment.GetEnvironmentVariable("CLIENT_SECRET") ?? string.Empty)
        .SetScope("email")
        .SetRedirectUrl("http://localhost:5128/api/oauth")
        .SetCustomQuery("grant_type", "authorization_code", QueryFor.GetAccessToken));

builder.Services.AddOAuths<TelegramOAuth, LinkOauthRepository, StrategyToken>(oAuths);


var log = new ConsoleLog(new ConsoleLogSettings
{
    ColorsEnabled = true
});
builder.Logging.ClearProviders();
builder.Logging.AddVostok(log);
builder.Services.AddSingleton<ILog>(log);

builder.Services.AddControllers().AddNewtonsoftJson();

builder.Services.AddHttpClient();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddTransient<ChatDb>();

// easyTg
builder.Services.AddTelegramCommands().AddTelegramContext();
builder.Services.AddTelegramBotWithController("https://8fd1b9be658e65.lhr.life",
    Environment.GetEnvironmentVariable("TG_TOKEN", EnvironmentVariableTarget.User) ??
    throw new ArgumentException("NOT HAVE TOKEN FOR BOT TG"));
builder.Services.AddScoped<IChatRepository, ChatRepository>();
builder.Services.AddScoped<IRecipeRepository, RecipeRepository>();
builder.Services.AddScoped<IChatContextRepository, ChatContextRepository>();
builder.Services.AddContext<AddingRecipeContext>(x => x
    .AddHandler<RecipeSetName>()
    .AddHandler<AddingIngredients>()
    .AddHandler<AddingInstruction>()
    .AddHandler<SaveRecipe>());
//
builder.Services.AddContext<CookContext>(x => x
    .AddHandler<ChoosingDish>()
    .AddHandler<Cooking>());

var app = builder.Build();
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