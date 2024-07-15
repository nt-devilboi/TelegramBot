using EasyOAuth;
using EasyOAuth.Abstraction;
using Telegram.Bot;
using TgBot.Domain.Entity;
using Vostok.Logging.Abstractions;

namespace TgBot.Infrastucture;

public class StrategyToken : IStrategyToken
{
    private readonly ILog _log;
    private ITelegramBotClient _bot;

    public StrategyToken(ILog log, ITelegramBotClient bot)
    {
        _log = log;
        _bot = bot;
    }

    public override async Task Execute(string token, OAuthEntity data)
    {
        var dataTotal = data as LinkOAuth;
        
        _log.Info($"{dataTotal.chatId}");
        await _bot.SendTextMessageAsync(long.Parse(dataTotal.chatId), "authosiziation is succesful");
    }
}