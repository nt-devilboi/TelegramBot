using System.Reflection;
using EasyOAuth;
using EasyOAuth.Builder;
using EasyOAuth.Extensions;
using EasyTgBot;
using TgBot;
using TgBot.Domain.Entity;
using TgBot.Infrastucture;
using TgBot.Infrastucture.DataBase;
using TgBot.Infrastucture.Repositories;
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
        .SetClientId("294027402115-glp7fra8naehdnacs0asu5n4m5vurngu.apps.googleusercontent.com")
        .SetClientSecret("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")
        .SetScope("email")
        .SetRedirectUrl("http://localhost:5128/api/oauth")
        .SetCustomQuery("grant_type", "authorization_code", QueryFor.GetAccessToken))
;


var log = new ConsoleLog(new ConsoleLogSettings()
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

builder.Services.AddOAuths<LinkOAuth,LinkOauthRepos, StrategyToken>(oAuths);

builder.Services.AddMediatR(cnf =>
{
    cnf.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
    cnf.Lifetime = ServiceLifetime.Singleton;
});

builder.Services.AddTransient<OAuthDb>();

//todo: было бы прикольна сделать это всё в одно FluetApi.
builder.Services.AddTelegramCommands();
builder.Services.AddTelegramBotWithController("https://1763-188-234-192-63.ngrok-free.app",
    Environment.GetEnvironmentVariable("TG_TOKEN") ?? throw new ArgumentException("NOT HAVE TOKEN FOR BOT TG"));

var app = builder.Build();
app.UseTgCommands();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/error");
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.MapControllers();
/*app.UseWhen(x => x.Request.Path.StartsWithSegments("/api"), c =>
{
c.UseMiddleware<IsTelegramChatMiddleWare>();
});*/
app.Run();