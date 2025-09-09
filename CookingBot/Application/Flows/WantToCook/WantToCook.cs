using System.Globalization;
using CookingBot.Application.Commands;
using CookingBot.Application.Commands.AddRecipe.Flow;
using CookingBot.Application.Commands.AddRecipe.Flow.ContextHandlers;
using CookingBot.Application.Flow;
using CookingBot.Application.Flows.AddRecipe.InContexts;
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

public class WantToCook(IRecipeRepository recipeRepository, IChatContextRepository chatContextRepository) : ICommand
{
    public string Trigger { get; } = Phrase.WantToCook.IWantToCook;
    public string Desc { get; }

    public async Task Execute(Update update, ITelegramBotClient bot, ChatContext context = null)
    {
        var cookContext =
            ContextFactory<CookPayload, TransactionServiceCook, CookContext>.Create(context);
        if (context.InPublic())
        {
            await bot.SendTextMessageAsync(update.Message.Chat.Id, "Сначала нужно зарегистрироваться");
            return;
        }

        if (!context.InUserAccount())
        {
            await bot.SendTextMessageAsync(update.Message.Chat.Id, "Ты делаешь, что-то другое");
            return;
        }


        var textRequest = update.AsRequestWithText();
        var recipes = await recipeRepository.Get(textRequest.GetChatId());
        if (recipes.Count == 0)
        {
            await bot.SendTextMessageAsync(textRequest.GetChatId(), "У тебя нету рецептов");
            return;
        }


        await bot.SendTextMessageAsync(textRequest.GetChatId(), Phrase.WantToCook.WhatDoYouWant,
            replyMarkup: new ReplyKeyboardMarkup(
                GetButtons(recipes)));

        context.State = (int)CookContext.ChoosingDish;
        await chatContextRepository.Upsert(context);
    }

    private IEnumerable<KeyboardButton> GetButtons(IReadOnlyList<Recipe> recipes)
    {
        foreach (var recipe in recipes)
        {
            var date = recipe.WasCookedLastTime?.ToString("dd.mm.yyyy");
            var stringData = date != null ? $"Готовилось {date}" : "Не готовил";
            if (recipe.WasCookedLastTime != null)
                yield return new KeyboardButton($"{ToUpperFirst(recipe.nameRecipe)}. {stringData}");
        }
    }


    private string ToUpperFirst(string str)
    {
        return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str);
    }
}