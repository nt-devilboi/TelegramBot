using System.Text.RegularExpressions;
using CookingBot.Application.Commands;
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

public partial class ChoosingDish(
    IRecipeRepository recipeRepository,
    IContextRepository contextRepository,
    ITelegramBotClient botClient)
    : ContextHandler<CookPayload, CookContext>
{
    [GeneratedRegex(@"^.+(?=\. )")]
    private static partial Regex TakeNameDish();

    protected override async Task Handle(Update update, DetailContext<CookPayload, CookContext> context)
    {
        var request = update.AsRequestWithText();
        var recipeName = TakeNameDish().Match(request.Value).Value;
        var recipe = await recipeRepository.Get(recipeName);
        if (recipe == null)
        {
            await botClient.SendTextMessageAsync(request.GetChatId(), $"Нету рецепта с названием {recipeName}");
            return;
        }

        var cook = new CookPayload(recipe.nameRecipe);
        context.UpdatePayload(cook);
        await botClient.SendTextMessageAsync(request.GetChatId(), $"Вот что нужно для блюда:");
        await botClient.SendTextMessageAsync(request.GetChatId(), recipe.GetIngredientsList());
        await botClient.SendTextMessageAsync(request.GetChatId(), $"Инструкция:\n {recipe.Instruction}");
        await botClient.SendTextMessageAsync(request.GetChatId(), "Скажешь как приготовишь",
            replyMarkup: new ReplyKeyboardMarkup
            ([
                [Phrase.WantToCook.ICooked]
            ]));

        context.NextState();
    }
}