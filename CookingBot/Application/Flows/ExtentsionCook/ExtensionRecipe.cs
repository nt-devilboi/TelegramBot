using CookingBot.Domain.Entity;

namespace CookingBot.Application.Flows.ExtentsionCook;

public static class ExtensionRecipe
{
    public static string GetIngredientsList(this Recipe x)
    {
        return string.Join("\n", x.Ingredients.Select((ing, i) =>
            $"{i + 1}. {ing.Key}: {ing.Value.Units} {ing.Value.Measurement}"));
    }
    
    public static (string name, uint unit, string measurement, bool isValid) AsIngredient(this string text)
    {
        var data = text.Split(" ", 3);
        if (data.Length != 3 || !uint.TryParse(data[1], out var unit))
        {
            return (default, default, default, false)!;
        }


        return (data[0], unit, data[2], true);
    }
}