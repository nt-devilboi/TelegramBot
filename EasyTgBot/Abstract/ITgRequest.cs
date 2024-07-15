using Telegram.Bot.Types;

namespace EasyTgBot.Abstract;

public interface ITgRequest
{
    public string CommandName { get; set; }
    public string ExtraData { get; set; }

    public Message Message { get; set; }

}