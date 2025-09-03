using EasyTgBot.Abstract;
using EasyTgBot.BaseCommand;

namespace EasyTgBot.ServiceCommand;

public class CommandCollection : ICommandCollection
{
    private readonly Dictionary<string, ICommand> _commands;
    private readonly List<InfoCommand> _infoCommands; //todo: вот бы это не хранить как отдельный класс)))

    public CommandCollection()
    {
        _infoCommands = new List<InfoCommand>();
        _commands = new Dictionary<string, ICommand>();

        Add(new Help(_infoCommands));
    }

    public void Add(ICommand command)
    {
        if (!_commands.TryAdd(command.Name, command))
            throw new ApplicationException($"command {command.Name} existed yet");


        var info = new InfoCommand { Info = $"{command.Name} - {command.Desc}" };
        _infoCommands.Add(info);
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