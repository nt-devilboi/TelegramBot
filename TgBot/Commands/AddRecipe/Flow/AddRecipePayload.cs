namespace TgBot.StatePayload;

public record AddRecipePayload
{
    public string nameRecipe { get; set; }
    public Dictionary<string, Ingredient> Ingredients { get; set; }
}

public class Ingredient
{
    public string Name { get; set; }
    public uint Count { get; set; }
}