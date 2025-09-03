using EasyOAuth.Abstraction;
using EasyTgBot.Abstract;
using EasyTgBot.Entity;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bots.Types;
using Vostok.Logging.Abstractions;
using InlineKeyboardMarkup = Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup;
using ParseMode = Telegram.Bot.Types.Enums.ParseMode;
using Update = Telegram.Bot.Types.Update;

namespace CookingBot.Commands;

public class OAuthGoogle : ICommand
{
    private readonly IOAuthClient _authClient;
    private readonly ILog _log;
    private readonly string _text = "Google";

    public OAuthGoogle(IOAuthClient authClient, ILog log)
    {
        _authClient = authClient;
        _log = log;
    }

    public string Name => "Авторизоваться через google";
    public string Desc => "Если ты еще не вошел нужно войти, чтоб я понимал, кто ты";

    public async Task Execute(Update update, ITelegramBotClient bot, ChatContext context = null)
    {
        var chatId = update.Message.Chat.Id;

        if (context.State != (int)ContextState.NotAuthenticated)
        {
            await bot.SendTextMessageAsync(chatId, "Ты уже авторизован");
            return;
        }

        _log.Info($"при созданий запроса chat id {chatId}");
        var bottons =
            new InlineKeyboardMarkup(InlineKeyboardButton.WithUrl(_text,
                await _authClient.GetOAuthRequest("google",
                    chatId.ToString()))); // добавляем вк. так сделанно ибо в будущем всё будет в базе данных хранится
        await bot.SendTextMessageAsync(chatId.ToString(), "выбери где авторизоваться", parseMode: ParseMode.Html,
            replyMarkup: bottons);
    }
}