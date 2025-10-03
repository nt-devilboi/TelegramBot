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
        DetailContext<RecipePayload, AddingRecipeContext> context)
    {
        var request = update.AsRequestWithText();


        if (request.Value == "Закончить") // если уж мы базарим про разные middleware, то можно сделать middleware, который берёт ответственность за переход на слеюудщий этап контекста.
        {
            await botClient.SendTextMessageAsync(request.GetChatId(), "Теперь напиши инструкцию");
            context.State.Continue();
            return;
        }

        var parsedText = Parse(request.Value);

        if (!parsedText.isValid)
        {
            await botClient.SendTextMessageAsync(context.ChatId, "Я так не понимаю. Напиши пожалуйста в таком стиле: \"яйца 3 штуки\"");
            return;
        }

        if (!AddIngredient(context, parsedText))
        {
            await botClient.SendTextMessageAsync(request.GetChatId(), "Ты это уже добавил или что-то пошло не так");
            return;
        }


        await botClient.SendTextMessageAsync(request.GetChatId(), "Добавил. Можешь добавить еще",
            replyMarkup: new ReplyKeyboardMarkup
            ([
                ["Закончить"]
            ]));
    }


    private static bool AddIngredient(DetailContext<RecipePayload, AddingRecipeContext> context,
        (string name, uint unit, string measurement, bool isValid) ingredient)
    {
        if (!context.TryGetPayload(out var payload) ||
            !payload.Ingredients.TryAdd(ingredient.name, new IngredientDetail(ingredient.unit, ingredient.measurement)))
            return false;

        context.UpdatePayload(payload);
        return true;
    }

    private (string name, uint unit, string measurement, bool isValid) Parse(string text)
    {
        var data = text.Split(" ", 3);
        if (data.Length != 3 || !uint.TryParse(data[1], out var unit))
        {
            return (default, default, default, false)!;
        }


        return (data[0], unit, data[2], true);
    }
}