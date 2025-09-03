using Moq;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace CookingBot.Tests;

public static class TelegramBotMockExtensions
{
    public static void VerifyMessageSent(this Mock<ITelegramBotClient> bot, string text, Times times,
        long? chatId = null)
    {
        bot.Verify(x => x.SendTextMessageAsync(
            chatId ?? It.IsAny<long>(),
            text,
            It.IsAny<int>(),
            It.IsAny<ParseMode>(),
            It.IsAny<IEnumerable<MessageEntity>>(),
            It.IsAny<bool>(),
            It.IsAny<bool>(),
            It.IsAny<bool>(),
            It.IsAny<int>(),
            It.IsAny<bool>(),
            It.IsAny<IReplyMarkup>(), It.IsAny<CancellationToken>()), times);
    }
}