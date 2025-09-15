using Microsoft.CodeAnalysis.CSharp.Syntax;
using Vostok.Logging.Abstractions;
using Vostok.Logging.Console;
using Vostok.Logging.File;
using Vostok.Logging.File.Configuration;
using Vostok.Logging.Microsoft;

namespace CookingBot;

internal static class ExtensionDi
{
    public static WebApplicationBuilder AddLog(this WebApplicationBuilder builder)
    {
        ILog? log;

        if (builder.Environment.IsDevelopment())
        {
            log = new ConsoleLog(new ConsoleLogSettings
            {
                ColorsEnabled = true,
            });
        }

        else
        {
            if (Directory.Exists("./logs")) Directory.CreateDirectory("./logs");
            log = new FileLog(new FileLogSettings()
            {
                FilePath = $"./logs/log{DateTime.Now:yyyy-M-d}.log",
            });
        }

        builder.Logging.ClearProviders();
        builder.Logging.AddVostok(log);
        builder.Services.AddSingleton<ILog>(log);
        return builder;
    }
}