using System.ComponentModel.DataAnnotations;
using EasyTgBot.Entity;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace EasyTgBot.Abstract;

public interface ICommand : IHandler
{
    public string Trigger { get; }

    public string Desc { get; }
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