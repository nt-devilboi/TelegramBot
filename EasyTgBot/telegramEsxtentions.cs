using EasyTgBot.Restored.Abstract;
using EasyTgBot.Result;
using Telegram.Bot.Types;

namespace EasyTgBot;

public static class TelegramExtensions
{
    //todo: упросить
    public static Result<ITgRequest?> Parse(this Message message)
    {
        var command = message.Text;
        if (string.IsNullOrEmpty(command)) return new Result<ITgRequest?>("you send empty");

        return new TgRequest
        {
            messageFromUser = command,
            Message = message
        };
    }
}