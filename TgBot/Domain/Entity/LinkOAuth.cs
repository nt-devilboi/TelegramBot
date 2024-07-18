using EasyOAuth;
using EasyOAuth.Abstraction;

namespace TgBot.Domain.Entity;

public class LinkOAuth : OAuthEntity
{
    public Guid Id { get; set; }
    public string chatId { get; set; }
}