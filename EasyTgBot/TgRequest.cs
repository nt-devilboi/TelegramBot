using EasyTgBot.Restored.Abstract;
using Telegram.Bot.Types;

namespace EasyTgBot;

public class TgRequest : ITgRequest
{
    public string messageFromUser { get; set; }
    public string ExtraData { get; set; }
    public Message Message { get; set; }
}