using EasyTgBot.Abstract;
using EasyTgBot.BaseCommand;

namespace EasyTgBot.ServiceCommand;

internal class CommandServiceRegistry : IServiceRegistry<ICommand>
{
    private readonly Dictionary<string, ICommand> _commands;

    public CommandServiceRegistry(IEnumerable<ICommand> commands)
    {
        _commands = new Dictionary<string, ICommand>();

        foreach (var command in commands)
        {
            Add(command);
        }
    }


    public void Add(ICommand command, string? key = null)
    {
        if (!_commands.TryAdd(command.Trigger, command))
            throw new ApplicationException($"command {command.Trigger} is exist yet");
    }

    public bool Contains(string commandName)
    {
        return _commands.ContainsKey(commandName);
    }

    public ICommand Get(string commandName)
    {
        if (!_commands.ContainsKey(commandName)) throw new ArgumentException($"this Command not Exists {commandName}");

        return _commands[commandName];
    }
}