using CookingBot.Application.Interfaces;
using EasyTgBot.Abstract;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CookingBot.Application.Flows.EditRecipe.InContext;

public class EditInstruction(IRecipeRepository recipeRepository, ITelegramBotClient botClient) : ContextHandler<ChoseRecipePayload, EditContext>
{
    protected override async Task Handle(Update update, DetailContext<ChoseRecipePayload, EditContext> context)
    {
        if (!context.TryGetPayload(out var payload)) return;

        var oldPayload = (await recipeRepository.Get(payload.NameRecipe))!; //  я уверен, так как в прошлом контексте это проверялось.

        oldPayload.Instruction = update.Message.Text;

        await recipeRepository.Upsert(oldPayload);

        await botClient.SendTextMessageAsync(context.ChatId, "Инструкцию изменил");
        context.Reset();
    }

    protected override async Task Enter(DetailContext<ChoseRecipePayload, EditContext> context)
    {
        if (!context.TryGetPayload(out var payload)) return;
        var recipe = await recipeRepository.Get(payload.NameRecipe);
        await botClient.SendTextMessageAsync(context.ChatId, "Хорошо давай изменим инструкцию \n сейчас она такая");
        await botClient.SendTextMessageAsync(context.ChatId, recipe!.Instruction);
        await botClient.SendTextMessageAsync(context.ChatId, "Напиши новую версию");
    }
}