using CookingBot.Application.Interfaces;
using CookingBot.Domain.Payloads;
using EasyTgBot.Abstract;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CookingBot.Application.Flows.EditRecipe.InContext;

public class EditIngredients(IRecipeRepository recipeRepository, ITelegramBotClient botClient)
    : ContextHandler<ChoseRecipePayload, EditContext>
{
    public static readonly (string delete, string add) Commands = ("Удали", "Добавь");

    protected override async Task Handle(Update update, DetailContext<ChoseRecipePayload, EditContext> context)
    {
        var (command, ingredient, isValid) = update.Message.Text;

        if (!isValid)
        {
            await botClient.SendTextMessageAsync(context.ChatId, "например, Напиши Удали\\добавь  сыр");
            return;
        }

        if (!context.TryGetPayload(out var payload)) return;
        var recipe = (await recipeRepository.Get(payload.NameRecipe))!;


        if (string.Compare(command, Commands.delete, StringComparison.OrdinalIgnoreCase) == 0)
        {
            if (recipe.Ingredients.Remove(ingredient))
            {
                await recipeRepository.Upsert(recipe);
                await botClient.SendTextMessageAsync(context.ChatId, "Удалил");
                return;
            }

            await botClient.SendTextMessageAsync(context.ChatId, "Такого ингредиента нету");
            return;
        }

        if (string.Compare(command, Commands.add, StringComparison.OrdinalIgnoreCase) == 0)
        {
            if (recipe.Ingredients.TryAdd(ingredient, new IngredientDetail(0, "штук")))
            {
                await recipeRepository.Upsert(recipe);
                await botClient.SendTextMessageAsync(context.ChatId, "Добавил");
                return;
            }

            await botClient.SendTextMessageAsync(context.ChatId, "Такой ингредиент уже есть");
        }
    }
}

public static class StringExtension
{
    public static void Deconstruct(this string s, out string command, out string name, out bool valid)
    {
        var parsedString = s.Split();
        if (parsedString.Length != 2 || string.IsNullOrEmpty(parsedString[0]) || string.IsNullOrEmpty(parsedString[1]))
        {
            command = null;
            name = null;
            valid = false;
            return;
        }

        valid = true;
        command = parsedString[0];
        name = parsedString[1];
    }

    public static void Deconstruct(this string s, out string command)
    {
        var parsedString = s.Split();
        command = parsedString[0];
    }
}