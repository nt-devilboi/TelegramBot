using EasyTgBot.Abstract;

namespace CookingBot.Domain.Payloads;

public record CookPayload(string NameRecipe) : BasePayload;