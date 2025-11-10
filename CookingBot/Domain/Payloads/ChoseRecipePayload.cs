using BotOrchestriX.Abstract;

namespace CookingBot.Domain.Payloads;

public record ChoseRecipePayload(string NameRecipe) : BasePayload;