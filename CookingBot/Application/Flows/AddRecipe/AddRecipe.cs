using CookingBot.Application.Commands;
using CookingBot.Application.Flows.AddRecipe.InContexts;
using EasyTgBot;
using EasyTgBot.Abstract;
using EasyTgBot.Entity;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CookingBot.Application.Flows.AddRecipe;

public class AddRecipe(ITelegramBotClient botClient)
    : Router, ICommand
{
    public string Trigger { get; } = StaticTrigger;
    public string Desc { get; }
    public static readonly string StaticTrigger = "Добавить рецепт";
    public Priority Priority { get; } = Priority.Command;

    public async Task Execute(Update request, ChatContext context)
    {
        var chatId = request.Message.Chat.Id;

        if (context.InPublic())
        {
            await botClient.SendTextMessageAsync(chatId, "Сначала нужно авторизоваться");
            return;
        }

        if (context.InUserAccount())
        {
            Route<AddingRecipeContext>(context);
        }
        else
        {
            await botClient.SendTextMessageAsync(chatId, "Ты находишься где-то не там");
        }
    }
}