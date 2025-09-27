using CookingBot.Domain.Entity;

namespace CookingBot.Application.Flows.ExtentsionCook;

public static class ExtensionRecipe
{
    public static string GetIngredientsList(this Recipe x)
    {
        return string.Join("\n", x.Ingredients.Select((ing, i) =>
            $"{i + 1}. {ing.Key}: {ing.Value.Units} {ing.Value.Measurement}"));
    }
}