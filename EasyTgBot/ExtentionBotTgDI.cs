using System.Reflection;
using System.Windows.Input;
using EasyTgBot.Abstract;
using EasyTgBot.controller;
using EasyTgBot.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;

namespace EasyTgBot;

public static class ExtensionBotTgDi
{
    public static void AddTelegramBotWithController<TMainMenuHandler>(this IServiceCollection serviceCollection,
        string host,
        string token) where TMainMenuHandler : class, IStrategyOnMenu
    {
        serviceCollection.AddMvc().AddApplicationPart(Assembly.GetAssembly(typeof(BotController)));
        var client = new TelegramBotClient(token);
        var webhook = $"{host}/api/message/update";
        client.SetWebhookAsync(webhook).Wait();
        serviceCollection.AddSingleton<ITelegramBotClient>(client);
        serviceCollection.AddScoped<IUpdateProcess, UpdateProcess>();
        serviceCollection.AddScoped<MessageHandler>();
        serviceCollection.AddScoped<IStrategyOnMenu, TMainMenuHandler>();
        serviceCollection.AddScoped<IContextFactory, ContextFactory>();
    }

    public static void AddTelegramDbContext<TDb>(this IServiceCollection serviceCollection) where TDb : ChatDb
    {
        serviceCollection.AddDbContext<TDb>();
        serviceCollection.AddDbContext<ChatDb, TDb>();

        serviceCollection.AddScoped<IChatRepository, ChatRepository>();
        serviceCollection.AddScoped<IContextRepository, ContextRepository>();
    }

    public static IServiceCollection AddTelegramCommands(this IServiceCollection serviceCollection)
    {
        var assembly = Assembly.GetCallingAssembly();
        var commandsTypes = GetCommandsFrom(assembly);
        foreach (var commandsType in commandsTypes)
        {
            serviceCollection.AddScoped<Command>(provider =>
                (Command)ActivatorUtilities.CreateInstance(provider, commandsType));
        }

        return serviceCollection;
    }


    private static Type[] GetCommandsFrom(Assembly assembly)
    {
        return assembly
            .GetTypes()
            .Where(t => t is { BaseType: not null, IsAbstract: false } &&
                        t.BaseType == typeof(Command)
            )
            .ToArray();
    }
}