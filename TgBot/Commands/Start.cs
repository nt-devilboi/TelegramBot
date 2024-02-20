using Telegram.Bot;
using TgBot.controller.BotController.Services;
using Vostok.Logging.Abstractions;

namespace TgBot.Commands;

public class Start : ICommand
{
    private ILog _log;

    public Start(ILog log)
    {
        _log = log;
    }

    public string Name { get; } = "/start";
    public string desc { get; } = "Start Telegram bot";

    public async Task Execute(IRequest? request, ITelegramBotClient bot)
    {
        var chatId = request.Message.Chat.Id;
        await bot.SendTextMessageAsync(chatId, "print /help to understand how to work with this");
    }
}