using System.Reflection;
using EasyOAuth;
using EasyOAuth.Builder;
using EasyOAuth.Extensions;
using EasyTgBot;
using EasyTgBot.Abstract;
using CookingBot;
using CookingBot.Application.Commands.AddRecipe.Flow;
using CookingBot.Application.Interfaces;
using CookingBot.Commands.AddRecipe.Flow;
using CookingBot.Domain.Entity;
using CookingBot.Infrastructure;
using CookingBot.Infrastructure.Repositories;
using CookingBot.Infrastucture;
using CookingBot.Infrastucture.DataBase;
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
            .SetCustomQuery("grant_type", "authorization_code", QueryFor.GetAccessToken))
    ;


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


builder.Services.AddOAuths<TelegramOAuth, LinkOauthRepository, StrategyToken>(oAuths);


builder.Services.AddMediatR(cnf =>
{
    cnf.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
    cnf.Lifetime = ServiceLifetime.Singleton;
});

builder.Services.AddTransient<ChatDb>();

builder.Services.AddTelegramCommands();
builder.Services.AddTelegramBotWithController("https://c74b1457247c85.lhr.life",
    Environment.GetEnvironmentVariable("TG_TOKEN", EnvironmentVariableTarget.User) ??
    throw new ArgumentException("NOT HAVE TOKEN FOR BOT TG"));
builder.Services.AddScoped<IChatRepository, ChatRepository>();
builder.Services.AddScoped<IRecipeRepository, RecipeRepository>();
builder.Services.AddScoped<IChatContextRepository, ChatContextRepository>();
builder.Services.AddScoped<IContextProcess, AddRecipeFlow>();
var app = builder.Build();
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseTgCommands();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();


app.MapControllers();

app.Run();