using CookingBot.Application.Interfaces;
using CookingBot.Domain.Entity;
using EasyTgBot.Abstract;
using EasyTgBot.Entity;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CookingBot.Application.Commands;

public class CheckMyRecipe(IRecipeRepository recipeRepository) : ICommand
{
    public string Trigger { get; } = "Покажи мой рецепты";
    public string Desc { get; }


    public async Task Execute(Update update, ITelegramBotClient bot, ChatContext context)
    {
        var recipes = await recipeRepository.Get(update.Message.Chat.Id);

        await bot.SendTextMessageAsync(update.Message.Chat.Id, GetRecipes(recipes));
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
            $" {i + 1}. {ing.Key}: {ing.Value.Count} {ing.Value.Measurement}"));
    }
}