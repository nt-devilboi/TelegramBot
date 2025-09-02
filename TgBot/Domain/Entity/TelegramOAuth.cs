using EasyOAuth.Abstraction;

namespace TgBot.Domain.Entity;

public class TelegramOAuth : OAuthEntity
{
    public string chatId { get; set; }
}