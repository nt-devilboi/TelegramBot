namespace CookingBot.Domain.Payloads;

public record RecipePayload
{
    public string nameRecipe { get; init; }
    public Dictionary<string, IngredientDetail> Ingredients { get; set; } = []; //todo: можно спокойно менять и тем самым json может тупо не соответсовать 

    public string Instruction { get; init; }
}

public record IngredientDetail(uint Count, string Measurement);