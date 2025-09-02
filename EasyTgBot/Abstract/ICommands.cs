using EasyTgBot.Restored.Abstract;

namespace EasyTgBot.Abstract;

public interface ICommandCollection
{
    public void Add(ICommand command);
    public bool Contains(string commandName);
    public ICommand Get(string commandName);
}