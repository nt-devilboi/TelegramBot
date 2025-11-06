using System.Collections;
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
    private const string Next = "Дальше";
    private const int Take = 3;


    private static string WhatDoYouWantToCook = "Что хочешь приготовить?";

    protected override async Task Handle(Update update, DetailContext<CookPayload, CookContext> context)
    {
        var request = update.CallbackQuery?.Data;
        if (request == null || !context.TryGetPayload(out var payload)) return;

        if ((request.StartsWith(Next) || request.StartsWith(Back)) &&
            int.TryParse(request.Split("_")[1], out var offset))
        {
            var recipes = await recipeRepository.GetByChatId(context.ChatId);

            await botClient.EditMessageReplyMarkupAsync(context.ChatId, payload.MessageId,
                replyMarkup: new InlineKeyboardMarkup(
                    GetButtons(recipes, offset)));

            return;
        }


        var recipe = await recipeRepository.GetById(long.Parse(request));
        if (recipe == null)
        {
            await botClient.SendTextMessageAsync(context.ChatId, $"Рецепт не нашел {request}");
            return;
        }

        var cook = new CookPayload(recipe.nameRecipe, payload.MessageId);
        context.UpdatePayload(cook);

        context.State.Continue();
    }

    protected override async Task Enter(DetailContext<CookPayload, CookContext> context)
    {
        var recipes = await recipeRepository.GetByChatId(context.ChatId);
        var message = await botClient.SendTextMessageAsync(context.ChatId, WhatDoYouWantToCook,
            replyMarkup: new InlineKeyboardMarkup(
                GetButtons(recipes)));

        context.UpdatePayload(new CookPayload("", message.MessageId));
    }


    private IEnumerable<InlineKeyboardButton[]> GetButtons(List<Recipe> recipes, int offset = 0, int take = Take)
    {
        recipes.Sort((x, y) =>
            (x.WasCookedLastTime ?? DateTime.MinValue).CompareTo(y.WasCookedLastTime ?? DateTime.MinValue));

        foreach (var recipe in recipes.Skip(offset).Take(take))
        {
            var date = recipe.WasCookedLastTime?.ToString("dd MMMM yyyy года", CultureInfo.GetCultureInfo("ru-RU"));
            var stringData = date != null ? $"Готовилось {date}" : "Не готовил";
            yield return
            [
                InlineKeyboardButton.WithCallbackData($"{ToUpperFirst(recipe.nameRecipe)}. \n {stringData}",
                    $"select_{recipe.Id}")
            ];
        }

        yield return
        [
            InlineKeyboardButton.WithCallbackData(Back, $"{Back}_{offset - take}"),
            InlineKeyboardButton.WithCallbackData(Next, $"{Next}_{offset + take}"),
        ];
    }

    private const string Back = "Назад";

    private string ToUpperFirst(string str)
    {
        return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str);
    }
}