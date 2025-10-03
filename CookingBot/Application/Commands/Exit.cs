using EasyTgBot;
using EasyTgBot.Abstract;
using EasyTgBot.Entity;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace CookingBot.Application.Commands;

public class Exit(IContextRepository contextRepository, ITelegramBotClient botClient) : ICommand
{
    public string Trigger { get; } = "Выйти";
    public string Desc { get; }
    public Priority Priority { get; } = Priority.SystemCommand;


    public async Task Execute(Update update, ChatContext context = null)
    {
        if (context.InUserAccount())
        {
            await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Ты уже в главном меню");
            return;
        }


        context.Payload = null;
        context.ToUserAccount();
        await contextRepository.Upsert(context);
        await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Теперь ты в главном меню");
    }
}