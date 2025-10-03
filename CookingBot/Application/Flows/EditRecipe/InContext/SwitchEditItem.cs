using CookingBot.Application.Flows.ExtentsionCook;
using CookingBot.Application.Interfaces;
using EasyTgBot.Abstract;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CookingBot.Application.Flows.EditRecipe.InContext;

public class SwitchEditItem(ITelegramBotClient botClient, IRecipeRepository repository)
    : ContextHandler<ChoseRecipePayload, EditContext>
{
    protected override async Task Handle(Update update, DetailContext<ChoseRecipePayload, EditContext> context)
    {
        if (ChooseEditItem.Buttons.instuction == update.Message.Text && context.TryGetPayload(out var payload))
        {
            var recipe = await repository.Get(payload.NameRecipe);
            await botClient.SendTextMessageAsync(context.ChatId, "Хорошо давай изменим инструкцию \n сейчас она такая");
            await botClient.SendTextMessageAsync(context.ChatId, recipe!.Instruction);
            await botClient.SendTextMessageAsync(context.ChatId, "Напиши новую версию");
            context.State.GoTo(EditContext.EditInstruction);
        }

        if (ChooseEditItem.Buttons.name == update.Message.Text && context.TryGetPayload(out payload))
        {
            var recipe = await repository.Get(payload.NameRecipe);
            await botClient.SendTextMessageAsync(context.ChatId, "Хорошо давай изменим название \n сейчас оно такое:");
            await botClient.SendTextMessageAsync(context.ChatId, recipe!.nameRecipe);
            await botClient.SendTextMessageAsync(context.ChatId, "Напиши новую версию");
            context.State.GoTo(EditContext.EditName);
        }
        
        if (ChooseEditItem.Buttons.ingredints == update.Message.Text && context.TryGetPayload(out payload))
        {
            var recipe = await repository.Get(payload.NameRecipe);
            await botClient.SendTextMessageAsync(context.ChatId, "Хорошо давай изменим ингредиенты \n сейчас они такие:");
            await botClient.SendTextMessageAsync(context.ChatId, recipe!.GetIngredientsList());
            await botClient.SendTextMessageAsync(context.ChatId, "что хочешь, чтоб я добавил или удалил");
            context.State.GoTo(EditContext.EditIngredients);
        }
    }
}