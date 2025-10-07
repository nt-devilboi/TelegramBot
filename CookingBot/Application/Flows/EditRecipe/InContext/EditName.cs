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
        context.Reset();
    }

    protected override async Task Enter(DetailContext<ChoseRecipePayload, EditContext> context)
    {
        if (!context.TryGetPayload(out var payload)) return;
        
        var recipe = await recipeRepository.Get(payload.NameRecipe);
        await botClient.SendTextMessageAsync(context.ChatId, "Хорошо давай изменим название \n сейчас оно такое:");
        await botClient.SendTextMessageAsync(context.ChatId, recipe!.nameRecipe);
        await botClient.SendTextMessageAsync(context.ChatId, "Напиши новую версию");
    }
}