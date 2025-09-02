using EasyTgBot.Entity;
using Telegram.Bot;

namespace EasyTgBot.Restored.Abstract;

public interface ICommand
{
    public string Name { get; }

    public string Desc { get; }
    public Task Execute(ITgRequest? request, ITelegramBotClient bot, ChatContext context = null);
}