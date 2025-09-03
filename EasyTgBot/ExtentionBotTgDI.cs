using System.Globalization;
using System.Reflection;
using EasyTgBot.Abstract;
using EasyTgBot.controller;
using EasyTgBot.ServiceCommand;
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

    public static void AddTelegramCommands(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<ICommandCollection, CommandCollection>();
    }

    //todo: нужен рефакторинг.
    public static void UseTgCommands(this IHost app)
    {
        var services = app.Services.CreateScope().ServiceProvider;

        var commandsCollection = services.GetService<ICommandCollection>();
        if (commandsCollection == null) throw new ApplicationException("NOT HAVE ADDED serviceCommands");

        var assembly = Assembly.GetCallingAssembly();
        var commandsTypes = GetCommandsFrom(assembly);
        foreach (var commandsType in commandsTypes)
        {
            var commandServices = services.GetFrom(commandsType);

            var command = assembly.CreateInstanceCommand(commandServices, commandsType.FullName);
            if (command == null) throw new ArgumentException($"command {commandsType.FullName} not create");

            commandsCollection.Add(command);
        }
    }

    private static List<object> GetFrom(this IServiceProvider serviceCollection, Type typeCommand)
    {
        return typeCommand.GetConstructors()[0]
            .GetParameters()
            .Select(parameter => serviceCollection
                .GetService(parameter.ParameterType))
            .ToList();
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