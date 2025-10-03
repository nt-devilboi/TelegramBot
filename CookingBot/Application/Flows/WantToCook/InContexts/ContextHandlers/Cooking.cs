using CookingBot.Application.Commands;
using CookingBot.Application.Interfaces;
using CookingBot.Domain.Payloads;
using EasyTgBot;
using EasyTgBot.Abstract;
using Telegram.Bot;
using Telegram.Bot.Types;
using Vostok.Logging.Abstractions;

namespace CookingBot.Application.Flows.WantToCook.InContexts.ContextHandlers;
//todo: насчёт upsert иногда будут команды без изменнеия context но upsert в любом случае выполнится, поэтому можно либо возвращать true false. detail context в abstract классе существует там можно знать про изменения. 
public class Cooking(
    IRecipeRepository recipeRepository,
    IContextRepository contextRepository,
    ITelegramBotClient botClient,
    ILog log) : ContextHandler<CookPayload, CookContext>
{
    protected override async Task Handle(Update update, DetailContext<CookPayload, CookContext> context)
    {
        log.Info("the user starts to cook");
        var request = update.AsRequestWithText();

        if (Phrase.WantToCook.ICooked == request.Value && context.TryGetPayload(out var payload))
        {
            var recipe = await recipeRepository.Get(payload.NameRecipe);
            if (recipe == null) throw new ApplicationException($"рецепта с названием {payload.NameRecipe} не нашлось");

            recipe.WasCookedLastTime = DateTime.Now.ToUniversalTime();

            await recipeRepository.Upsert(recipe);
            await botClient.SendTextMessageAsync(request.GetChatId(), "Я запомнил, когда ты приготовил");
            context.Reset();
        }


        log.Info($"the user {update.Message.From.Username} ends to cook");
    }
}