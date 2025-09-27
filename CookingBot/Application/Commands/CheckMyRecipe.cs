using CookingBot.Application.Interfaces;
using CookingBot.Domain.Entity;
using EasyTgBot.Abstract;
using EasyTgBot.Entity;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CookingBot.Application.Flows;

public class CheckMyRecipe(IRecipeRepository recipeRepository, ITelegramBotClient botClient) : ICommand
{
    public string Trigger { get; } = "Покажи мой рецепты";
    public string Desc { get; }


    public Priority Priority { get; } = Priority.Command;

    public async Task Execute(Update update, ChatContext context)
    {
        var recipes = await recipeRepository.Get(update.Message.Chat.Id);

        await botClient.SendTextMessageAsync(update.Message.Chat.Id, GetRecipes(recipes));
    }


    private static string GetRecipes(IReadOnlyList<Recipe> recipes)
    {
        return string.Join("\n\n",
            recipes.Select(x =>
                $"{x.nameRecipe}:\n" +
                $"{GetIngredientsList(x)}" +
                $"\n{x.Instruction}"));
    }

    private static string GetIngredientsList(Recipe x)
    {
        return string.Join("\n", x.Ingredients.Select((ing, i) =>
            $" {i + 1}. {ing.Key}: {ing.Value.Units} {ing.Value.Measurement}"));
    }
}