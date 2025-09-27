using CookingBot.Domain.Payloads;
using EasyTgBot;
using EasyTgBot.Abstract;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace CookingBot.Application.Flows.AddRecipe.InContexts.ContextHandlers;

public class AddingIngredients(ITelegramBotClient botClient)
    : ContextHandler<RecipePayload, AddingRecipeContext>
{
    protected override async Task Handle(Update update,
        DetailContext<RecipePayload, AddingRecipeContext> recipeContext)
    {
        var request = update.AsRequestWithText();
        var text = request.Value;
        if (request.Value == "Закончить")
        {
            await botClient.SendTextMessageAsync(request.GetChatId(), "Теперь давай инструкцию");
            recipeContext.NextState();
            return;
        }

        if (AddIngredient(recipeContext, text))
        {
            await botClient.SendTextMessageAsync(request.GetChatId(), "Ты это уже добавил или что-то пошло не так");
            return;
        }


        await botClient.SendTextMessageAsync(request.GetChatId(), "Добавить еще?",
            replyMarkup: new ReplyKeyboardMarkup
            ([
                ["Закончить"]
            ]));
    }


    private static bool AddIngredient(DetailContext<RecipePayload, AddingRecipeContext> context, string text)
    {
        if (!context.TryGetPayload(out var payload) ||
            !payload.Ingredients.TryAdd(text, new IngredientDetail(0, "штук"))) return false;


        var ingredient = payload.Ingredients[text];

        payload.Ingredients[text] = ingredient;

        context.UpdatePayload(payload);
        return true;
    }
}