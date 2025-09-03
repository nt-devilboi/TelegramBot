using EasyTgBot.Abstract;
using Telegram.Bot.Types;

namespace EasyTgBot;

public static class ExtensionUpdate
{
    public static long GetChatId<T>(this ITgRequest<T> request) =>
        request.Update.Message!.Chat.Id;

    public static TextRequest AsTextRequest(this Update update) => new()
    {
        Update = update,
        Value = update.Message.Text
    };
}