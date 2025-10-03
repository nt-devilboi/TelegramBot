using CookingBot.Application.Interfaces;
using EasyTgBot.Abstract;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace CookingBot.Application.Flows.EditRecipe.InContext;

public class ChooseEditItem(ITelegramBotClient botClient, IRecipeRepository recipeRepository)
    : ContextHandler<ChoseRecipePayload, EditContext>
{
    public static (string name, string instuction, string ingredints) Buttons = ("Название", "Инструкцию", "Ингредиенты");

    protected override async Task Handle(Update update, DetailContext<ChoseRecipePayload, EditContext> context)
    {
        var nameRecipe = update.Message.Text;
        if (nameRecipe == null)
        {
            await botClient.SendTextMessageAsync(context.ChatId, "Как-то пусто");
            return;
        }

        var recipe = await recipeRepository.Get(nameRecipe);
        if (recipe == null)
        {
            await botClient.SendTextMessageAsync(context.ChatId, "Такого рецепта нету");
            return;
        }

        context.UpdatePayload(new ChoseRecipePayload(nameRecipe));
        context.State.Continue();

        await botClient.SendTextMessageAsync(context.ChatId, "Что хочешь изменить?",
            replyMarkup: new ReplyKeyboardMarkup([Buttons.name, Buttons.instuction, Buttons.ingredints]));
    }
}