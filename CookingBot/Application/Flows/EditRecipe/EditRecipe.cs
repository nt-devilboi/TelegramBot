using CookingBot.Application.Commands;
using EasyTgBot.Abstract;
using EasyTgBot.Entity;
using Telegram.Bot.Types;

namespace CookingBot.Application.Flows.EditRecipe;

public class EditRecipe : Router, ICommand
{
    public string Trigger { get; } = StaticTrigger;
    public Priority Priority { get; } = Priority.Command;

    public static string StaticTrigger = "Редактировать рецепт";

    public string Desc { get; }

    public async Task Execute(Update update, ChatContext context)
    {
        Route<EditContext>(context);
    }
}