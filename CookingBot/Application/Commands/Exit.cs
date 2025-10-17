using EasyTgBot;
using EasyTgBot.Abstract;
using EasyTgBot.Entity;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CookingBot.Application.Commands;

public class Exit(IContextRepository contextRepository, ITelegramBotClient botClient) : Command
{
    public override string Trigger { get; } = "Выйти";
    public string Desc { get; }
    public override Priority Priority { get; } = Priority.SystemCommand;

    public override async Task Execute(Update update, ChatContext context = null)
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