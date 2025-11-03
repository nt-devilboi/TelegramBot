using CookingBot.Application.Flows.AddRecipe;
using CookingBot.Application.Flows.EditRecipe;
using CookingBot.Application.Flows.WantToCook;
using CookingBot.Application.Interfaces;
using EasyTgBot.Abstract;
using EasyTgBot.Entity;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace CookingBot;

public class MainMenuHandler(
    IRecipeRepository repository,
    ITelegramBotClient botClient,
    ITriggerProvider triggerProvider) : IStrategyOnMenu
{
    public async Task Handle(ChatContext context)
    {
        var recipe = await repository.Get(context.ChatId);
        if (recipe.Count != 0)
        {
            await botClient.SendTextMessageAsync(context.ChatId, "Можешь выполнять эти команды",
                replyMarkup: GetAvailableCommand());
        }
        else
        {
            await botClient.SendTextMessageAsync(context.ChatId, "Добавь рецепт",
                replyMarkup: new ReplyKeyboardMarkup(AddRecipe.StaticTrigger));
        }
    }

    private IReplyMarkup GetAvailableCommand()
    {
        var routers = triggerProvider.GetAll().Select(x => new KeyboardButton(x.Value));
        return new ReplyKeyboardMarkup(routers);
    }
}