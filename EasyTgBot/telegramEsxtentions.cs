using EasyTgBot.Abstract;
using EasyTgBot.Result;
using Telegram.Bot.Types;

namespace EasyTgBot;

public static class TelegramExtensions
{
    public static Result<ITgRequest?> Parse(this Message message)
    {
        var command = message.Text.Split(' ');
        if (command.Length == 0) return new Result<ITgRequest?>("you send empty");
        if (command[0][0] != '/') return new Result<ITgRequest?>("command start with '/'");
        var name = command[0];
        var extraData = command.Length == 2 ? command[1] : null;      
        
        return new TgTgRequest()
        {
            CommandName = name,
            ExtraData = extraData,
            Message = message
        };
    }
}
