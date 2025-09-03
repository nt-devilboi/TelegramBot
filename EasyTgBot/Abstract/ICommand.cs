using EasyTgBot.Entity;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace EasyTgBot.Abstract;

public interface ICommand
{
    public string Name { get; }

    public string Desc { get; }
    public Task Execute(Update update, ITelegramBotClient bot, ChatContext context = null);
}