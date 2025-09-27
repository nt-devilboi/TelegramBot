using System.Globalization;
using CookingBot.Application.Commands;
using CookingBot.Application.Flows.WantToCook.InContexts;
using CookingBot.Application.Interfaces;
using CookingBot.Domain.Entity;
using CookingBot.Domain.Payloads;
using EasyTgBot;
using EasyTgBot.Abstract;
using EasyTgBot.Entity;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace CookingBot.Application.Flows.WantToCook;

public class WantToCook(
    IRecipeRepository recipeRepository,
    ITelegramBotClient botClient) : ICommand
{
    public string Trigger { get; } = Phrase.WantToCook.IWantToCook;
    public string Desc { get; }

    public Priority Priority { get; } = Priority.Command;


    public async Task Execute(Update update, ChatContext context = null)
    {
        if (context.InPublic())
        {
            await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Сначала нужно зарегистрироваться");
            return;
        }

        if (!context.InUserAccount())
        {
            await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Ты делаешь, что-то другое");
            return;
        }


        var textRequest = update.AsRequestWithText();
        var recipes = await recipeRepository.Get(textRequest.GetChatId());
        if (recipes.Count == 0)
        {
            await botClient.SendTextMessageAsync(textRequest.GetChatId(), "У тебя нету рецептов");
            return;
        }


        await botClient.SendTextMessageAsync(textRequest.GetChatId(), Phrase.WantToCook.WhatDoYouWant,
            replyMarkup: new ReplyKeyboardMarkup(
                GetButtons(recipes)));

        context.State = (int)CookContext.ChoosingDish;
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