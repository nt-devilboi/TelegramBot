using Telegram.Bot.Types;

namespace EasyTgBot.Restored.Abstract;

public interface ITgRequest
{
    public string messageFromUser { get; set; }
    public string ExtraData { get; set; }

    public Message Message { get; set; }
}