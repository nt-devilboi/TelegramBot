using EasyTgBot.Abstract;
using Telegram.Bot.Types;

namespace EasyTgBot;

public class TgRequest : ITgRequest
{
    public string CommandName { get; set; }
    public string ExtraData { get; set; }
    public Message Message { get; set; }
}