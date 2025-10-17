using CookingBot.Application.Flows.ExtentsionCook;
using CookingBot.Application.Interfaces;
using CookingBot.Domain.Payloads;
using EasyTgBot.Abstract;
using EasyTgBot.Entity;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CookingBot.Application.Flows.EditRecipe.InContext;

public class EditIngredients(IRecipeRepository recipeRepository, ITelegramBotClient botClient)
    : ContextHandler<ChoseRecipePayload, EditContext>
{
    public static readonly (string delete, string add) Commands = ("Удали", "Добавь");

    protected override async Task Handle(Update update, DetailContext<ChoseRecipePayload, EditContext> context)
    {
        var (opcode, data) = Parse(update.Message.Text);

        if (!context.TryGetPayload(out var payload)) return;
        var recipe = (await recipeRepository.Get(payload.NameRecipe))!;


        if (string.Compare(opcode, Commands.delete, StringComparison.OrdinalIgnoreCase) == 0)
        {
            if (recipe.Ingredients.Remove(data))
            {
                await recipeRepository.Upsert(recipe);
                await botClient.SendTextMessageAsync(context.ChatId, "Удалил");
                return;
            }

            await botClient.SendTextMessageAsync(context.ChatId, "Такого ингредиента нету");
            return;
        }

        if (string.Compare(opcode, Commands.add, StringComparison.OrdinalIgnoreCase) == 0)
        {
            var ingredient = data.AsIngredient();
            if (!ingredient.isValid)
            {
                await botClient.SendTextMessageAsync(context.ChatId, "Я понимаю только так: \"добавь сыр 300 грамм\"");
                return;
            }


            if (recipe.Ingredients.TryAdd(ingredient.name,
                    new IngredientDetail(ingredient.unit, ingredient.measurement)))
            {
                await recipeRepository.Upsert(recipe);
                await botClient.SendTextMessageAsync(context.ChatId, "Добавил");
                return;
            }

            await botClient.SendTextMessageAsync(context.ChatId, "Такой ингредиент уже есть");
        }

        else
        {
            await botClient.SendTextMessageAsync(context.ChatId, "Например, Напиши Удали\\добавь сыр");
        }
    }

    private (string opcode, string data) Parse(string s)
    {
        var result = s.Split(" ");
        return (result[0], string.Join(" ", result[1..]));
    }

    protected override async Task Enter(DetailContext<ChoseRecipePayload, EditContext> context)
    {
        if (!context.TryGetPayload(out var payload)) return;
        var recipe = await recipeRepository.Get(payload.NameRecipe);
        await botClient.SendTextMessageAsync(context.ChatId, "Хорошо давай изменим ингредиенты \n сейчас они такие:");
        await botClient.SendTextMessageAsync(context.ChatId, recipe!.GetIngredientsList());
        await botClient.SendTextMessageAsync(context.ChatId, "что хочешь, чтоб я добавил или удалил");
    }
}