using EasyTgBot.Abstract;
using EasyTgBot.Entity;
using EasyTgBot.Restored.Abstract;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using TgBot.Commands.AddRecipe.Flow;
using TgBot.Domain.Entity;
using TgBot.StatePayload;

namespace TgBot.HandlerContext;

public class AddRecipeFlow(IChatContextRepository chatContextRepository) : IContextProcess
{
    public async Task Handle(ITgRequest request, ITelegramBotClient telegramBotClient, ChatContext context)
    {
        if (request.messageFromUser == "Покажи результат")
        {
            await telegramBotClient.SendTextMessageAsync(request.Message.Chat.Id,
                string.IsNullOrEmpty(context.Payload) ? "У меня нету данных" : context.Payload);
            return;
        }

        var recipeContext = RecipeContext.Create(context);

        switch (recipeContext.State)
        {
            case RecipeContext.AddingName:
                await SetName(request, telegramBotClient, recipeContext);
                break;
            case RecipeContext.AddingIngredient:
                await AddIngredient(request, telegramBotClient, recipeContext);
                break;
            default:
                Error(request, context, telegramBotClient);
                break;
        }

        if (recipeContext.WasChanged) await chatContextRepository.Upsert(context);
    }

    private async Task AddIngredient(ITgRequest request, ITelegramBotClient bot, RecipeContext context)
    {
        if (request.Message.Text == "Закончить")
        {
            await bot.SendTextMessageAsync(request.Message.Chat.Id, "Теперь давай инструкцию");
            context.NextState();
            return;
        }

        context.Payload.Ingredients ??= new Dictionary<string, Ingredient>();


        context.Payload.Ingredients.TryAdd(request.Message.Text,
            new Ingredient { Name = request.Message.Text, Count = 0 });

        context.Payload.Ingredients[request.Message.Text].Count++;

        context.SaveChanges();
        await bot.SendTextMessageAsync(request.Message.Chat.Id, "Добавить еще",
            replyMarkup: new ReplyKeyboardMarkup
            ([
                ["Закончить"]
            ]));
    }

    private void Error(ITgRequest request, ChatContext context, ITelegramBotClient telegramBotClient)
    {
        telegramBotClient.SendTextMessageAsync(request.Message.Chat.Id, "Я еще ничего не знаю");
    }


    private async Task SetName(ITgRequest request, ITelegramBotClient botClient, RecipeContext context)
    {
        var payload = new AddRecipePayload
        {
            nameRecipe = request.messageFromUser
        };

        context.SetPayload(payload);

        context.SaveChanges();
        context.NextState();
        await botClient.SendTextMessageAsync(request.Message.Chat.Id, "Какие ингредиенты");
    }
}