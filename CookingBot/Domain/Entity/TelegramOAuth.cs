using EasyOAuth.Abstraction;

namespace CookingBot.Domain.Entity;

public class TelegramOAuth : OAuthEntity
{
    public long chatId { get; init; }
}