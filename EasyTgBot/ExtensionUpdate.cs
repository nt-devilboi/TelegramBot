using EasyTgBot.Abstract;
using Telegram.Bot.Types;

namespace EasyTgBot;

public static class ExtensionUpdate
{
    public static long GetChatId<T>(this TelegramRequest<T> request) =>
        request.Update.Message!.Chat.Id;

    public static TelegramRequest<string> AsRequestWithText(this Update update) => new(update.Message.Text, update);
}