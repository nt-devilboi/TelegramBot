using EasyTgBot.Entity;
using Telegram.Bot.Types;

namespace EasyTgBot.Abstract;

public abstract class Command : IHandler
{
    public abstract string Trigger { get; }
    public virtual Priority Priority { get; } = Priority.Command;
    public abstract Task Execute(Update update, ChatContext context);
}

public enum Priority
{
    Command,
    SystemCommand
}

public interface IHandler
{
    public Task Execute(Update update, ChatContext context);
}