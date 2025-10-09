using System.Globalization;
using CookingBot.Application.Commands;
using CookingBot.Application.Flows.WantToCook.InContexts;
using CookingBot.Application.Interfaces;
using CookingBot.Domain.Entity;
using EasyTgBot;
using EasyTgBot.Abstract;
using EasyTgBot.Entity;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace CookingBot.Application.Flows.WantToCook;

public class WantToCook(
    IRecipeRepository recipeRepository,
    ITelegramBotClient botClient) : Router, ICommand
{
    public string Trigger { get; } = StaticTrigger;
    public string Desc { get; }

    public static readonly string StaticTrigger = "Хочу приготовить";
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


        Route<CookContext>(context);
    }
}