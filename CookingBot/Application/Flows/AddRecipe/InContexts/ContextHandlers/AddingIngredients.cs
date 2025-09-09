using CookingBot.Application.Commands.AddRecipe.Flow;
using CookingBot.Application.Commands.AddRecipe.Flow.ContextHandlers;
using CookingBot.Application.Flow;
using CookingBot.Application.Flows.AddRecipe.InContexts;
using CookingBot.Domain.Payloads;
using EasyTgBot;
using EasyTgBot.Abstract;
using EasyTgBot.Entity;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace CookingBot.Application.Flows.AddRecipe.Contexts.ContextHandlers;

public class AddingIngredients(IChatContextRepository chatContextRepository) : IContextHander
{
    public async Task Handle(Update update, ITelegramBotClient bot, ChatContext context)
    {
        var recipeContext =
            ContextFactory<RecipePayload, TransactionServiceRecipe, AddingRecipeContext>.Create(context);
        var request = update.AsRequestWithText();
        var text = request.Value;
        if (request.Value == "Закончить")
        {
            await bot.SendTextMessageAsync(request.GetChatId(), "Теперь давай инструкцию");
            recipeContext.NextState();
            await chatContextRepository.Upsert(context);
            return;
        }

        AddIngredient(recipeContext, text);


        await bot.SendTextMessageAsync(request.GetChatId(), "Добавить еще",
            replyMarkup: new ReplyKeyboardMarkup
            ([
                ["Закончить"]
            ]));

        await chatContextRepository.Upsert(context);
    }


    private static void AddIngredient(DetailContext<RecipePayload, AddingRecipeContext> context, string text)
    {
        // пока есть косяк с инкасуляцией. мы меняет payload здесь, но не меняем json. сейчас это решает SaveChanges.
        if (!context.TryGetPayload(out var payload)) return;

        payload.Ingredients.TryAdd(text, new IngredientDetail(0, "штук"));
        var ingredient = payload.Ingredients[text];
        ingredient = ingredient with { Count = ingredient.Count + 1 };

        payload.Ingredients[text] = ingredient;

        context.UpdatePayload(payload);
    }
}