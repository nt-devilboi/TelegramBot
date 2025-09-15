using System.Text.RegularExpressions;
using CookingBot.Application.Commands;
using CookingBot.Application.Flow;
using CookingBot.Application.Flows.ExtentsionCook;
using CookingBot.Application.Interfaces;
using CookingBot.Domain.Payloads;
using EasyTgBot;
using EasyTgBot.Abstract;
using EasyTgBot.Entity;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace CookingBot.Application.Flows.WantToCook.InContexts.ContextHandlers;

public partial class ChoosingDish(IRecipeRepository recipeRepository, IChatContextRepository chatContextRepository)
    : IContextHander
{
    public async Task Handle(Update update, ITelegramBotClient bot, ChatContext context)
    {
        var request = update.AsRequestWithText();
        var cookContext = ContextFactory<CookPayload, TransactionServiceCook, CookContext>.Create(context);
        var recipeName = TakeNameDish().Match(request.Value).Value;
        var recipe = await recipeRepository.Get(recipeName);
        if (recipe == null)
        {
            await bot.SendTextMessageAsync(request.GetChatId(), $"нету рецепта с названием {recipeName}");
        }
        var cook = new CookPayload(recipe.nameRecipe);
        cookContext.UpdatePayload(cook);
        await bot.SendTextMessageAsync(request.GetChatId(), $"Вот что нужно для блюда:");
        await bot.SendTextMessageAsync(request.GetChatId(), recipe.GetIngredientsList());
        await bot.SendTextMessageAsync(request.GetChatId(), $"Инструкция:\n {recipe.Instruction}");
        await bot.SendTextMessageAsync(request.GetChatId(), "Скажешь как приготовишь",
            replyMarkup: new ReplyKeyboardMarkup
            ([
                [Phrase.WantToCook.ICooked]
            ]));

        cookContext.NextState();
        await chatContextRepository.Upsert(context);
    }

    [GeneratedRegex(@"^.+(?=\. )")]
    private static partial Regex TakeNameDish();
}