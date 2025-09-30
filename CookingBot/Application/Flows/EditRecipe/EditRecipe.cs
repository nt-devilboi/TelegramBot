using System.Globalization;
using CookingBot.Application.Flows.EditRecipe.InContext;
using CookingBot.Application.Interfaces;
using CookingBot.Domain.Entity;
using EasyTgBot.Abstract;
using EasyTgBot.Entity;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace CookingBot.Application.Flows.EditRecipe;

public class EditRecipe(IRecipeRepository repository, ITelegramBotClient botClient) : ICommand
{
    public string Trigger { get; } = "Редактировать рецепт";
    public Priority Priority { get; } = Priority.Command;

    public string Desc { get; }

    public async Task Execute(Update update, ChatContext context)
    {
        var recipes = await repository.Get(context.ChatId);

        await botClient.SendTextMessageAsync(context.ChatId, "Какой рецепт хочешь отредактировать",
            replyMarkup: new ReplyKeyboardMarkup(GetButtons(recipes)));
        context.State = (int)EditContext.ChooseEditItem;
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