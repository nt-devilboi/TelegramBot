using System.Globalization;
using CookingBot.Application.Interfaces;
using CookingBot.Domain.Entity;
using CookingBot.Infrastructure.Repositories;
using EasyTgBot;
using EasyTgBot.Abstract;
using EasyTgBot.Entity;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace CookingBot.Application.Commands.AddRecipe;

public class WantToCook(IRecipeRepository recipeRepository) : ICommand
{
    public string Trigger { get; } = "Хочу приготовить";
    public string Desc { get; }

    public async Task Execute(Update update, ITelegramBotClient bot, ChatContext context = null)
    {
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


        var textRequest = update.AsTextRequest();
        var recipes = await recipeRepository.Get(textRequest.GetChatId());
        if (recipes.Count == 0)
        {
            await bot.SendTextMessageAsync(textRequest.GetChatId(), "У тебя нету рецептов");
            return;
        }


        await bot.SendTextMessageAsync(textRequest.GetChatId(), "Что хочешь приготовить?",
            replyMarkup: new ReplyKeyboardMarkup(
                GetButtons(recipes)));
    }   

    private IEnumerable<KeyboardButton> GetButtons(IReadOnlyList<Recipe> recipes)
    {
        var currentTime = DateTime.Now;

        foreach (var recipe in recipes)
        {
            if (recipe.WasCookedLastTime != null)
                yield return new KeyboardButton(
                    $"{recipe.nameRecipe}. Готовился {(currentTime - recipe.WasCookedLastTime).Value.Days} назад"); //нету слова день, так как нужны разные окончания
            else
            {
                
                yield return new KeyboardButton(
                    $"{ToUpperFirst(recipe.nameRecipe)}. Не готовил");
            }
        }
    }

    private string ToUpperFirst(string str)
    {
        return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str);
    }
}