namespace CookingBot.Application.Commands;

public static class Triggers
{
    public static TriggerAddRecipe AddRecipe => new TriggerAddRecipe();
}

public static class Phrase
{
    public static RecipePhrase Recipe => new RecipePhrase();
    public static WantToCookPhrase WantToCook => new WantToCookPhrase();
}

public class WantToCookPhrase
{
    public string WhatDoYouWant = "Что хочешь приготовить?";
    public string IWantToCook = "Хочу приготовить";
    public string ICooked = "Приготовил";
}

public class RecipePhrase
{
    public readonly string AskIngredients = "Какие ингредиенты";
    public readonly string Save = "Сохранил!";
}

public class TriggerAddRecipe
{
    public readonly string AddRecipe = "Добавить рецепт";
    public readonly string ShowResult = "Покажи результат";
}