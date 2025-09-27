using EasyTgBot.Entity;
using Telegram.Bot.Types;

namespace EasyTgBot.Abstract;

public interface IContextHandler
{
    internal Task Handle(Update update, ChatContext context, IContextFactory? contextFactory = null);
}