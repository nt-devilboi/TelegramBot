namespace EasyTgBot.Abstract;

public interface IServiceRegistry<TTrigger>
{
    public void Add(TTrigger command, string? key = null);
    public bool Contains(string commandName);
    public TTrigger Get(string commandName);
}