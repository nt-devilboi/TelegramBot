using CookingBot.Application.Interfaces;
using EasyTgBot.Abstract;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CookingBot.Application.Flows.EditRecipe.InContext;

public class EditName(IRecipeRepository recipeRepository, ITelegramBotClient botClient) : ContextHandler<ChoseRecipePayload, EditContext>
{
    protected override async Task Handle(Update update, DetailContext<ChoseRecipePayload, EditContext> context)
    {
        if (!context.TryGetPayload(out var payload)) return;
        var oldPayload = (await recipeRepository.Get(payload.NameRecipe))!;

        oldPayload.nameRecipe = update.Message.Text;

        await recipeRepository.Upsert(oldPayload);

        await botClient.SendTextMessageAsync(context.ChatId, "Сохранил изменение");        
        context.ToUserAccount();
    }
}