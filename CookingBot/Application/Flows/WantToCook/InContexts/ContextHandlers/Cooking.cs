using CookingBot.Application.Commands;
using CookingBot.Application.Flows.ExtentsionCook;
using CookingBot.Application.Interfaces;
using CookingBot.Domain.Payloads;
using EasyTgBot;
using EasyTgBot.Abstract;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Vostok.Logging.Abstractions;

namespace CookingBot.Application.Flows.WantToCook.InContexts.ContextHandlers;

//todo: насчёт upsert иногда будут команды без изменнеия context но upsert в любом случае выполнится, поэтому можно либо возвращать true false. detail context в abstract классе существует там можно знать про изменения. 
public class Cooking(
    IRecipeRepository recipeRepository,
    ITelegramBotClient botClient,
    ILog log) : ContextHandler<CookPayload, CookContext>
{
    private string Cooked = "Приготовил";

    protected override async Task Handle(Update update, DetailContext<CookPayload, CookContext> context)
    {
        log.Info("the user starts to cook");
        var request = update.AsRequestWithText();

        if (Cooked == request.Value && context.TryGetPayload(out var payload))
        {
            var recipe = (await recipeRepository.Get(payload.NameRecipe))!;

            recipe.WasCookedLastTime = DateTime.Now.ToUniversalTime();

            await recipeRepository.Upsert(recipe);
            await botClient.SendTextMessageAsync(request.GetChatId(), "Я запомнил, когда ты приготовил");
            context.Reset();
        }


        log.Info($"the user {update.Message.From.Username} ends to cook");
    }

    protected override async Task Enter(DetailContext<CookPayload, CookContext> context)
    {
        if (!context.TryGetPayload(out var payload)) return;

        var recipe = await recipeRepository.Get(payload.NameRecipe);
        await botClient.SendTextMessageAsync(context.ChatId, $"Вот что нужно для блюда:");
        await botClient.SendTextMessageAsync(context.ChatId, recipe.GetIngredientsList());
        await botClient.SendTextMessageAsync(context.ChatId, $"Инструкция:\n {recipe.Instruction}");
        await botClient.SendTextMessageAsync(context.ChatId, "Скажешь как приготовишь",
            replyMarkup: new ReplyKeyboardMarkup
            ([
                [Cooked]
            ]));
    }
}