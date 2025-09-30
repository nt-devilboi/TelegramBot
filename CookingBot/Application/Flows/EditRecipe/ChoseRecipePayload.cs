using EasyTgBot.Abstract;

namespace CookingBot.Application.Flows.EditRecipe;

public record ChoseRecipePayload(string NameRecipe) : BasePayload;