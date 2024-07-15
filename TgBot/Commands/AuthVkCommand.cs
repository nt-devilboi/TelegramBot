using System.ComponentModel;
using EasyOAuth.Abstraction;
using EasyTgBot.Abstract;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Vostok.Logging.Abstractions;

namespace TgBot.Commands.VkCommands;

[Description("Auth By vk")]
public class AuthVkCommand : CommandTgBase
{
    public string Name { get; } = "/vk";
    public override string Desc { get; } = "auth by vk";

    private readonly IOAuthClient _authClient;
    private readonly string _text = "Vk";
    private ILog _log;
    public AuthVkCommand(IOAuthClient authClient, ILog log)
    {
        _authClient = authClient;
        _log = log;
    }

    public override async Task Execute(ITgRequest? request, ITelegramBotClient bot)
    {
        var chatId = request.Message.Chat.Id;
        
        _log.Info($"при созданий запроса chat id {chatId}");
        var bottons =
            new InlineKeyboardMarkup(InlineKeyboardButton.
                WithUrl(_text, await _authClient.GetOAuthRequest("google", chatId.ToString()))); // добавляем вк. так сделанно ибо в будущем всё будет в базе данных хранится
        await bot.SendTextMessageAsync(chatId.ToString(), "выбери где авторизоваться", parseMode: ParseMode.Html,
            replyMarkup: bottons);
    }
}