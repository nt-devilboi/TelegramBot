namespace CookingBot.Commands.AddRecipe.Flow;

public enum AddingRecipeStateContext
{
    ReturnToMenu = 1,
    AddingName = 10000,
    AddingIngredient,
    AddingInstruction,
    SaveRecipe
}