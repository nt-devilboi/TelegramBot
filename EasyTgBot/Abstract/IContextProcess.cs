using EasyTgBot.Entity;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace EasyTgBot.Abstract;

public interface IContextProcess
{
    Task Handle(Update update,ITelegramBotClient telegramBotClient, ChatContext context);
}