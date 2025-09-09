using CookingBot.Application.Commands;
using CookingBot.Application.Flow;
using CookingBot.Application.Interfaces;
using CookingBot.Domain.Entity;
using CookingBot.Domain.Payloads;
using EasyTgBot;
using EasyTgBot.Abstract;
using EasyTgBot.Entity;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CookingBot.Application.Flows.AddRecipe.InContexts.ContextHandlers;

public class RecipeSetName(IChatContextRepository chatContextRepository, IRecipeRepository recipeRepository)
    : IContextHander
{
    public async Task Handle(Update update, ITelegramBotClient bot, ChatContext context)
    {
        if (string.IsNullOrEmpty(update.Message.Text)) return;


        if (update.Message.Text == Triggers.AddRecipe.ShowResult)
        {
            await bot.SendTextMessageAsync(update.Message.Chat.Id,
                string.IsNullOrEmpty(context.Payload) ? "У меня нету данных" : context.Payload);
            return;
        }

        var recipeContext =
            ContextFactory<RecipePayload, TransactionServiceRecipe, AddingRecipeContext>.Create(context);


        await SetName(update.AsRequestWithText(), bot, recipeContext);

        await chatContextRepository.Upsert(context);
    }


    private async Task SetName(TelegramRequest<string> request, ITelegramBotClient botClient,
        DetailContext<RecipePayload, AddingRecipeContext> context)
    {
        var payload = new RecipePayload
        {
            nameRecipe = request.Value.ToLower()
        };

        context.UpdatePayload(payload)
            .NextState();


        await botClient.SendTextMessageAsync(request.GetChatId(), Phrase.Recipe.AskIngredients);
    }
}