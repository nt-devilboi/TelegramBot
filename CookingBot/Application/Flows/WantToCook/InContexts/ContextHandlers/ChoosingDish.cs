using System.Globalization;
using System.Text.RegularExpressions;
using CookingBot.Application.Interfaces;
using CookingBot.Domain.Entity;
using CookingBot.Domain.Payloads;
using EasyTgBot;
using EasyTgBot.Abstract;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace CookingBot.Application.Flows.WantToCook.InContexts.ContextHandlers;

public partial class ChoosingDish(
    IRecipeRepository recipeRepository,
    ITelegramBotClient botClient)
    : ContextHandler<CookPayload, CookContext>
{
    [GeneratedRegex(@"^.+(?=\. )")]
    private static partial Regex TakeNameDish();

    private static string WhatDoYouWantToCook = "Что хочешь приготовить?";

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

        context.State.Continue();
    }

    protected override async Task Enter(DetailContext<CookPayload, CookContext> context)
    {
        var recipes = await recipeRepository.Get(context.ChatId);
        await botClient.SendTextMessageAsync(context.ChatId, WhatDoYouWantToCook,
            replyMarkup: new ReplyKeyboardMarkup(
                GetButtons(recipes)));
    }


    private IEnumerable<KeyboardButton> GetButtons(IReadOnlyList<Recipe> recipes)
    {
        foreach (var recipe in recipes)
        {
            var date = recipe.WasCookedLastTime?.ToString("dd MMMM yyyy года", CultureInfo.GetCultureInfo("ru-RU"));
            var stringData = date != null ? $"Готовилось {date}" : "Не готовил";
            yield return new KeyboardButton($"{ToUpperFirst(recipe.nameRecipe)}. {stringData}");
        }
    }

    private string ToUpperFirst(string str)
    {
        return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str);
    }
}