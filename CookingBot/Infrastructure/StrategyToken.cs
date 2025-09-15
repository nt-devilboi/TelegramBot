using CookingBot.Domain.Entity;
using EasyOAuth.Abstraction;
using EasyTgBot.Abstract;
using EasyTgBot.Entity;
using Telegram.Bot;
using Vostok.Logging.Abstractions;

namespace CookingBot.Infrastructure;

public class StrategyToken : EasyOAuth.Abstraction.StrategyToken
{
    private readonly ITelegramBotClient _bot;
    private readonly IChatContextRepository _chatContextRepository;
    private readonly IChatRepository _chatRepository;
    private readonly ILog _log;

    public StrategyToken(ILog log, ITelegramBotClient bot, IChatRepository chatRepository,
        IChatContextRepository chatContextRepository)
    {
        _log = log;
        _bot = bot;
        _chatRepository = chatRepository;
        _chatContextRepository = chatContextRepository;
    }

    public override async Task Execute(string token, OAuthEntity data)
    {
        var telegramOAuth = data as TelegramOAuth;
        var chatContext = ChatContext.CreateInAccountContext(telegramOAuth.chatId);
        var chat = new Chat
        {
            token = token,
            Id = telegramOAuth.chatId,
        };

        await _chatRepository.Add(chat);
        await _chatContextRepository.Upsert(chatContext);

        _log.Info($"{telegramOAuth.chatId}");


        await _bot.SendTextMessageAsync(telegramOAuth.chatId, "authosiziation is succesful");
    }
}