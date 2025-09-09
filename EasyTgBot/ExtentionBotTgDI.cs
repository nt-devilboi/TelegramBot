using System.Globalization;
using System.Reflection;
using EasyTgBot.Abstract;
using EasyTgBot.controller;
using EasyTgBot.ServiceCommand;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;

namespace EasyTgBot;

public static class ExtensionBotTgDI
{
    public static void AddTelegramBotWithController(this IServiceCollection serviceCollection, string host,
        string token)
    {
        serviceCollection.AddMvc().AddApplicationPart(Assembly.GetAssembly(typeof(BotController)));
        var client = new TelegramBotClient(token);
        var webhook = $"{host}/api/message/update";
        client.SetWebhookAsync(webhook).Wait();
        serviceCollection.AddSingleton<ITelegramBotClient>(client);
    }

    public static IServiceCollection AddTelegramCommands(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IServiceRegistry<ICommand>, CommandServiceRegistry>();
        var assembly = Assembly.GetCallingAssembly();
        var commandsTypes = GetCommandsFrom(assembly);
        foreach (var commandsType in commandsTypes)
        {
            serviceCollection.AddScoped<ICommand>(provider =>
                (ICommand)ActivatorUtilities.CreateInstance(provider, commandsType));
        }

        return serviceCollection;
    }

    public static void AddTelegramContext(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IServiceRegistry<IContextHander>, ContextServiceRegistry>();
    }


    private static Type[] GetCommandsFrom(Assembly assembly)
    {
        return assembly
            .GetTypes()
            .Where(t => t.GetInterface(typeof(ICommand).ToString()) != null)
            .ToArray();
    }

    private static ICommand? CreateInstanceCommand(this Assembly assembly, List<object> servicesForCommand,
        string fullName)
    {
        return assembly.CreateInstance(fullName,
            false,
            BindingFlags.Public | BindingFlags.Instance,
            null,
            servicesForCommand.ToArray(), CultureInfo.CurrentCulture,
            null) as ICommand;
    }
}