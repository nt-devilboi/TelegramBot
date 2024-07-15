namespace EasyTgBot.Abstract;

public interface ICommandCollection
{
    public void Add(ICommandTg commandTg);
    public bool Contains(string commandName);
    public ICommandTg Get(string commandName);
}