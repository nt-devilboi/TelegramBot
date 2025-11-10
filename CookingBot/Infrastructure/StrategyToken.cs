using BotOrchestriX.Abstract;
using BotOrchestriX.Entity;
using CookingBot.Application.Flows.AddRecipe;
using CookingBot.Domain.Entity;
using EasyOAuth.Abstraction;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using Vostok.Logging.Abstractions;

namespace CookingBot.Infrastructure;

public class StrategyToken(
    ILog log,
    ITelegramBotClient bot,
    IContextRepository contextRepository)
    : EasyOAuth.Abstraction.StrategyToken
{
    public override async Task Execute(string token, OAuthEntity data)
    {
        var telegramOAuth = data as TelegramOAuth; // по идей можно это исправить и сделать без этого
        var chatContext = ChatContext.CreateInAccountContext(telegramOAuth.chatId);
        var firstName = (await bot.GetChatAsync(telegramOAuth.chatId)).FirstName;


        await contextRepository.Upsert(chatContext);

        log.Info($"Token was linked with {telegramOAuth.chatId}");


        await bot.SendTextMessageAsync(telegramOAuth.chatId, "Теперь ты можешь выполнять эти команды",
            replyMarkup: new ReplyKeyboardMarkup([AddRecipe.StaticTrigger]));
    }
}