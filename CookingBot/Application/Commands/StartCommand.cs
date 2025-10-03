using EasyTgBot.Abstract;
using EasyTgBot.Entity;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace CookingBot.Application.Commands;

public class StartCommand(ITelegramBotClient botClient) : ICommand
{
    public Priority Priority { get; } = Priority.Command;

    public async Task Execute(Update update, ChatContext context)
    {
        await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Привет сначала нужно авторизоваться",
            replyMarkup: new ReplyKeyboardMarkup("Авторизоваться"));
    }

    public string Trigger { get; } = "/start";
    public string Desc { get; }
}