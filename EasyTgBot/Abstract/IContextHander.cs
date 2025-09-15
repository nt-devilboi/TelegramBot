using EasyTgBot.Entity;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace EasyTgBot.Abstract;

public interface IContextHander
{
    Task Handle(Update update, ITelegramBotClient bot, ChatContext context);
}