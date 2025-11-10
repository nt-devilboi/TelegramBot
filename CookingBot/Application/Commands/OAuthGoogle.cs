using BotOrchestriX;
using BotOrchestriX.Abstract;
using BotOrchestriX.Entity;
using EasyOAuth.Abstraction;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using Vostok.Logging.Abstractions;
using InlineKeyboardMarkup = Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup;
using ParseMode = Telegram.Bot.Types.Enums.ParseMode;
using Update = Telegram.Bot.Types.Update;

namespace CookingBot.Application.Commands;

public class OAuthGoogle(IOAuthClient authClient, ILog log, ITelegramBotClient botClient)
    : Command
{
    public static readonly string StaticTrigger = "Авторизоваться";
    private readonly string _text = "Google";

    public override string Trigger { get; } = StaticTrigger;
    public string Desc => "Если ты еще не вошел нужно войти, чтоб я понимал кто ты";

    public Priority Priority { get; } = Priority.Command;


    public override async Task Execute(Update update, ChatContext context)
    {
        var chatId = update.Message.Chat.Id;

        if (context.IsMenu())
        {
            await botClient.SendTextMessageAsync(chatId, "Ты уже авторизован");
            return;
        }


        log.Info($"при созданий запроса chat id {chatId}");
        var bottons =
            new InlineKeyboardMarkup(InlineKeyboardButton.WithUrl(_text,
                await authClient.GetOAuthRequest("google",
                    chatId.ToString())));

        await botClient.SendTextMessageAsync(chatId.ToString(), "выбери где авторизоваться", parseMode: ParseMode.Html,
            replyMarkup: bottons);
    }
}