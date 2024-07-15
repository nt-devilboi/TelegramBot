using EasyTgBot.Abstract;
using EasyTgBot.BaseCommand;

namespace EasyTgBot.ServiceCommand;

public class CommandCollection : ICommandCollection
{
    private readonly Dictionary<string, ICommandTg> _commands;
    private readonly List<InfoCommand> _infoCommands; //todo: вот бы это не хранить как отдельный класс)))

    public CommandCollection()
    {
        _infoCommands = new List<InfoCommand>();
        _commands = new Dictionary<string, ICommandTg>();

        Add(new HelpCommand(_infoCommands));
    }

    public void Add(ICommandTg commandTg)
    {
        if (_commands.ContainsKey(commandTg.Name))
        {
            throw new ApplicationException($"command {commandTg.Name} existed yet");
        }
        
        if (commandTg.Name[0] != '/')
        {
            throw new AggregateException("command not start with '/'");
        }
        
        _commands.Add(commandTg.Name, commandTg);
        var info = new InfoCommand() { Info = $"{commandTg.Name} - {commandTg.Desc}" }; // выглядит как кринж.
        _infoCommands.Add(info);
    }

    public bool Contains(string commandName)
    {
        return _commands.ContainsKey(commandName);
    }

    public ICommandTg Get(string commandName)
    {
        if (!_commands.ContainsKey(commandName))
        {
            throw new ArgumentException($"this Command not Existed {commandName}");
        }

        return _commands[commandName];
    }
}