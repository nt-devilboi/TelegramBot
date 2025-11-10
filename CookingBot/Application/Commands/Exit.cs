using BotOrchestriX;
using BotOrchestriX.Abstract;
using BotOrchestriX.Entity;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CookingBot.Application.Commands;

public class Exit(ITelegramBotClient botClient) : Command
{
    public override string Trigger { get; } = "Выйти";
    public string Desc { get; }
    public override Priority Priority { get; } = Priority.SystemCommand;

    public override async Task Execute(Update update, ChatContext context = null)
    {
        if (context.IsMenu())
        {
            await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Ты уже в главном меню");
            return;
        }

        context.Payload = null;
        context.ToMenu();
        await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Теперь ты в главном меню");
    }
}