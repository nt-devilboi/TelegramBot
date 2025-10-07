using CookingBot.Domain.Payloads;
using EasyTgBot;
using EasyTgBot.Abstract;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CookingBot.Application.Flows.AddRecipe.InContexts.ContextHandlers;

public class AddingInstruction(ITelegramBotClient botClient)
    : ContextHandler<RecipePayload, AddingRecipeContext>
{
    protected override async Task Handle(Update update,
        DetailContext<RecipePayload, AddingRecipeContext> context)
    {
        var request = update.AsRequestWithText();
        if (context.TryGetPayload(out var payload))
        {
            payload = payload with { Instruction = request.Value };
            context.UpdatePayload(payload).State.Continue();
        }
    }

    protected override async Task Enter(DetailContext<RecipePayload, AddingRecipeContext> context)
    {
        await botClient.SendTextMessageAsync(context.ChatId, "Теперь напиши инструкцию");
    }
}
// а норм ли что инфу про следующие иструкцию даёт прошошлая. не слишком ли появляетяс зависимость.