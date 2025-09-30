using EasyTgBot.Entity;
using Telegram.Bot.Types;

namespace EasyTgBot.Abstract;

public interface ICommand : IHandler
{
    public string Trigger { get; }

    public string Desc { get; }
    public Priority Priority { get; }
    public abstract Task Execute(Update update, ChatContext context);
}

public enum Priority
{
    Command,
    SystemCommand
}

public interface IHandler
{
    public Priority Priority { get; }
    public Task Execute(Update update, ChatContext context);
}