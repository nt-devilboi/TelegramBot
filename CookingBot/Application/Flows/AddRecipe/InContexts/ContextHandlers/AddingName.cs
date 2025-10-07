using CookingBot.Domain.Payloads;
using EasyTgBot;
using EasyTgBot.Abstract;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CookingBot.Application.Flows.AddRecipe.InContexts.ContextHandlers;

public class AddingName(ITelegramBotClient botClient)
    : ContextHandler<RecipePayload, AddingRecipeContext>
{
    protected override async Task Handle(Update update,
        DetailContext<RecipePayload, AddingRecipeContext> context)
    {
        if (string.IsNullOrEmpty(update.Message.Text)) return;


        if (update.Message.Text == "Покажи результат")
        {
            if (context.TryGetPayload(out var payload))
            {
                await botClient.SendTextMessageAsync(update.Message.Chat.Id, payload.nameRecipe);
            }
            else
            {
                await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Нету данных");
            }

            return;
        }


        await SetName(update.AsRequestWithText(), context);
    }

    protected override async Task Enter(DetailContext<RecipePayload, AddingRecipeContext> context)
    {
        await botClient.SendTextMessageAsync(context.ChatId, $"Сначала скажи мне название рецепта");
    }


    private async Task SetName(TelegramRequest<string> request,
        DetailContext<RecipePayload, AddingRecipeContext> context)
    {
        var payload = new RecipePayload
        {
            nameRecipe = request.Value.ToLower()
        };

        context.UpdatePayload(payload).State.Continue();
    }
}