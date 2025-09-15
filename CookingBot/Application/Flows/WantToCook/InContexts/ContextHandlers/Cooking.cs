using CookingBot.Application.Commands;
using CookingBot.Application.Flow;
using CookingBot.Application.Interfaces;
using CookingBot.Domain.Payloads;
using EasyTgBot;
using EasyTgBot.Abstract;
using EasyTgBot.Entity;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CookingBot.Application.Flows.WantToCook.InContexts.ContextHandlers;

public class Cooking(IRecipeRepository recipeRepository, IChatContextRepository contextRepository) : IContextHander
{
    
    public async Task Handle(Update update, ITelegramBotClient bot, ChatContext context)
    {
        var request = update.AsRequestWithText();
        var cookContext = ContextFactory<CookPayload, TransactionServiceCook, CookContext>.Create(context);

        if (Phrase.WantToCook.ICooked == request.Value && cookContext.TryGetPayload(out var payload))
        {
            var recipe = await recipeRepository.Get(payload.NameRecipe);
            if (recipe == null) throw new ApplicationException($"рецепта с названием {payload.NameRecipe} не нашлось");

            recipe.WasCookedLastTime = DateTime.Now.ToUniversalTime();

            await recipeRepository.Upsert(recipe);
            await bot.SendTextMessageAsync(request.GetChatId(), "Я запомнил, когда ты приготовил");
            cookContext.ToUserAccount();

            await contextRepository.Upsert(context);
        }
    }
}