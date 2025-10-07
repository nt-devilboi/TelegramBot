using EasyTgBot.Abstract;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace CookingBot.Application.Flows.EditRecipe.InContext;

public class SwitchEditItem(ITelegramBotClient botClient)
    : ContextHandler<ChoseRecipePayload, EditContext>
{
    private static (string name, string instuction, string ingredints) Buttons = ("Название", "Инструкцию",
        "Ингредиенты");

    protected override async Task Handle(Update update, DetailContext<ChoseRecipePayload, EditContext> context)
    {
        if (Buttons.instuction == update.Message.Text && context.TryGetPayload(out var payload))
        {
            context.State.GoTo(EditContext.EditInstruction);
        }

        if (Buttons.name == update.Message.Text && context.TryGetPayload(out payload))
        {
            context.State.GoTo(EditContext.EditName);
        }

        if (Buttons.ingredints == update.Message.Text && context.TryGetPayload(out payload))
        {
            context.State.GoTo(EditContext.EditIngredients);
        }
    }

    protected override async Task Enter(DetailContext<ChoseRecipePayload, EditContext> context)
    {
        await botClient.SendTextMessageAsync(context.ChatId, "Что хочешь изменить?",
            replyMarkup: new ReplyKeyboardMarkup([Buttons.name, Buttons.instuction, Buttons.ingredints]));
    }
}