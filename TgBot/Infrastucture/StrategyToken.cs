using EasyOAuth.Abstraction;
using EasyTgBot.Entity;
using EasyTgBot.Restored.Abstract;
using Telegram.Bot;
using TgBot.Domain.Entity;
using TgBot.Infrastucture.DataBase;
using Vostok.Logging.Abstractions;

namespace TgBot.Infrastucture;

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
        var dataTotal = data as TelegramOAuth;
        var chatContext = new ChatContext
        {
            State = (int)ContextState.Menu,
            Id = Guid.NewGuid()
        };
        var chat = new Chat
        {
            token = token,
            ChatId = dataTotal.chatId,
            Id = Guid.NewGuid(),
            ChatContext = chatContext
        };

        await _chatRepository.Add(chat);


        _log.Info($"{dataTotal.chatId}");


        await _bot.SendTextMessageAsync(long.Parse(dataTotal.chatId), "authosiziation is succesful");
    }
}