using System.Globalization;
using CookingBot.Application.Interfaces;
using CookingBot.Domain.Entity;
using EasyTgBot.Abstract;
using EasyTgBot.Entity;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace CookingBot.Application.Flows.EditRecipe.InContext;

public class ChooseEditRecipe(ITelegramBotClient botClient, IRecipeRepository recipeRepository)
    : ContextHandler<ChoseRecipePayload, EditContext>
{
    
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
    }

    protected override async Task Enter(DetailContext<ChoseRecipePayload, EditContext> context)
    {
        var recipes = await recipeRepository.Get(context.ChatId);

        await botClient.SendTextMessageAsync(context.ChatId, "Какой рецепт хочешь отредактировать",
            replyMarkup: new ReplyKeyboardMarkup(GetButtons(recipes)));
    }
    
    private IEnumerable<KeyboardButton> GetButtons(IReadOnlyList<Recipe> recipes)
    {
        return recipes.Select(recipe => new KeyboardButton($"{ToUpperFirst(recipe.nameRecipe)}"));
    }

    private static string ToUpperFirst(string str)
    {
        return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str);
    }
}