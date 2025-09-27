using System.Reflection;
using EasyTgBot.Abstract;
using EasyTgBot.controller;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;

namespace EasyTgBot;

public static class ExtensionBotTgDi
{
    public static void AddTelegramBotWithController(this IServiceCollection serviceCollection, string host,
        string token)
    {
        serviceCollection.AddMvc().AddApplicationPart(Assembly.GetAssembly(typeof(BotController)));
        var client = new TelegramBotClient(token);
        var webhook = $"{host}/api/message/update";
        client.SetWebhookAsync(webhook).Wait();
        serviceCollection.AddSingleton<ITelegramBotClient>(client);
        serviceCollection.AddScoped<IUpdateProcess, UpdateProcess>();
        serviceCollection.AddScoped<IMessageHandler, MessageHandler>();
        serviceCollection.AddScoped<IContextFactory, ContextFactory>();
    }

    public static IServiceCollection AddTelegramCommands(this IServiceCollection serviceCollection)
    {
        var assembly = Assembly.GetCallingAssembly();
        var commandsTypes = GetCommandsFrom(assembly);
        foreach (var commandsType in commandsTypes)
        {
            serviceCollection.AddScoped<ICommand>(provider =>
                (ICommand)ActivatorUtilities.CreateInstance(provider, commandsType));
        }

        return serviceCollection;
    }


    private static Type[] GetCommandsFrom(Assembly assembly)
    {
        return assembly
            .GetTypes()
            .Where(t => t.GetInterface(typeof(ICommand).ToString()) != null)
            .ToArray();
    }
}