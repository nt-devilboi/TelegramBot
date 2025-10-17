using EasyTgBot.Abstract;
using EasyTgBot.Entity;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace CookingBot.Application.Commands;

public class StartHandler(ITelegramBotClient botClient) : Command
{
    public Priority Priority { get; } = Priority.Command;

    public override async Task Execute(Update update, ChatContext context)
    {
        await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Привет сначала нужно авторизоваться",
            replyMarkup: new ReplyKeyboardMarkup("Авторизоваться"));
    }

    public override string Trigger { get; } = "/start";
    public string Desc { get; }
}