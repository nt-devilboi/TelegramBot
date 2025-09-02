using EasyTgBot.Entity;
using EasyTgBot.Restored.Abstract;
using Telegram.Bot;

namespace EasyTgBot.Abstract;

public interface IContextProcess
{
    Task Handle(ITgRequest request, ITelegramBotClient telegramBotClient, ChatContext context);
}