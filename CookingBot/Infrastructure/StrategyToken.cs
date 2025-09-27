using CookingBot.Domain.Entity;
using EasyOAuth.Abstraction;
using EasyTgBot.Abstract;
using EasyTgBot.Entity;
using Telegram.Bot;
using Vostok.Logging.Abstractions;

namespace CookingBot.Infrastructure;

public class StrategyToken(
    ILog log,
    ITelegramBotClient bot,
    IChatRepository chatRepository,
    IContextRepository contextRepository)
    : EasyOAuth.Abstraction.StrategyToken
{
    public override async Task Execute(string token, OAuthEntity data)
    {
        var telegramOAuth = data as TelegramOAuth;
        var chatContext = ChatContext.CreateInAccountContext(telegramOAuth.chatId);
        var chat = new Chat
        {
            Token = token,
            Id = telegramOAuth.chatId,
        };

        await chatRepository.Add(chat);
        await contextRepository.Upsert(chatContext);

        log.Info($"Token was linked with {telegramOAuth.chatId}");


        await bot.SendTextMessageAsync(telegramOAuth.chatId, "authosiziation is succesful");
    }
}