using EasyTgBot.Abstract;

namespace EasyTgBot.ServiceCommand;

internal class ContextServiceRegistry : IServiceRegistry<IContextHander>
{
    private readonly Dictionary<string, IContextHander> _commands = new Dictionary<string, IContextHander>();
    private readonly Dictionary<Type, IContextHander> _contextProcesses;

    public ContextServiceRegistry(IEnumerable<HandlerInfo> handlers)
    {
        foreach (var handler in handlers)
        {
            Add(handler.ContextHander, handler.number);
        }
    }

    public void Add(IContextHander command, string? key = null)
    {
        _commands.Add(key!, command);
    }

    public bool Contains(string context)
    {
        return _commands.ContainsKey(context);
    }

    public IContextHander Get(string context)
    {
        if (!_commands.ContainsKey(context))
            throw new ArgumentException($"this {context} not found in {typeof(ContextServiceRegistry)}");

        return _commands[context];
    }
}