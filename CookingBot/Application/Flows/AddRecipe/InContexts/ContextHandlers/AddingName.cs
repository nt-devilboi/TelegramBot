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


        if (update.Message.Text == Triggers.AddRecipe.ShowResult)
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


        await SetName(update.AsRequestWithText(), botClient, context);
    }


    private async Task SetName(TelegramRequest<string> request, ITelegramBotClient botClient,
        DetailContext<RecipePayload, AddingRecipeContext> context)
    {
        var payload = new RecipePayload
        {
            nameRecipe = request.Value.ToLower()
        };

        context.UpdatePayload(payload)
            .State.Continue();


        await botClient.SendTextMessageAsync(request.GetChatId(), Phrase.Recipe.AskIngredients);
    }
}