using CookingBot.Application.Interfaces;
using CookingBot.Commands.AddRecipe.Flow;
using CookingBot.Domain.Entity;
using CookingBot.Domain.Payloads;
using EasyTgBot;
using EasyTgBot.Abstract;
using EasyTgBot.Entity;
using Telegram.Bot; 
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace CookingBot.Application.Commands.AddRecipe.Flow;
public class AddRecipeFlow(IChatContextRepository chatContextRepository, IRecipeRepository recipeRepository)
    : IContextProcess
{
    public async Task Handle(Update update, ITelegramBotClient telegramBotClient, ChatContext context)
    {
        if (string.IsNullOrEmpty(update.Message.Text)) return;

    
        if (update.Message.Text == "Покажи результат")
        {
            await telegramBotClient.SendTextMessageAsync(update.Message.Chat.Id,
                string.IsNullOrEmpty(context.Payload) ? "У меня нету данных" : context.Payload);
            return;
        }

        var recipeContext = RecipeContext<RecipePayload, AddingRecipeStateContext>.Create(context);

        switch (recipeContext.State)
        {
            case AddingRecipeStateContext.AddingName:
                await SetName(update.AsTextRequest(), telegramBotClient, recipeContext);
                break;
            case AddingRecipeStateContext.AddingIngredient:
                await AddIngredient(update.AsTextRequest(), telegramBotClient, recipeContext);
                break;
            case AddingRecipeStateContext.AddingInstruction:
                await AddInstruction(update.AsTextRequest(), telegramBotClient, recipeContext);
                break;
            case AddingRecipeStateContext.SaveRecipe:
                await SaveRecipe(update.AsTextRequest(), telegramBotClient, recipeContext);
                break;
            default:
                Error(update.AsTextRequest(), context, telegramBotClient);
                break;
        }

        if (recipeContext.WasChanged) await chatContextRepository.Upsert(context);
    }

    private async Task SaveRecipe(TextRequest TextRequest, ITelegramBotClient telegramBotClient,
        IDetailContext<RecipePayload, AddingRecipeStateContext> recipeContext)
    {
        if (TextRequest.Value != "Сохранить")
        {
            await telegramBotClient.SendTextMessageAsync(TextRequest.GetChatId(), "Я тебя не понимаю");
            return;
        }
        var payload = recipeContext.Payload;

        var recipe =
            new Recipe() // если вдруг будет появляется возможность создать этот класс еще где-то, то тогда наверное это имеет смысл перенети в конструктор или в extension, так как уже будет код повторяться, а пока ожидается, что это будет только здесь.
                {
                    nameRecipe = payload.nameRecipe,
                    Ingredients = payload.Ingredients,
                    Instruction = payload.Instruction,
                    Id = Guid.NewGuid(),
                    ChatId = TextRequest.GetChatId()
                };

        await recipeRepository.Upsert(recipe);

        await telegramBotClient.SendTextMessageAsync(TextRequest.GetChatId(), "Сохранил!");
    }


    private async Task AddInstruction(TextRequest TextRequest, ITelegramBotClient bot,
        IDetailContext<RecipePayload, AddingRecipeStateContext> recipeContext)
    {
        //todo: помнишь тот прикольный with в andromeda тестах еще где баг? там это было сделано сугубо потому что быра работа с классом и нужно было повторить логику рекодр у клааса
        recipeContext.PostPayload(recipeContext.Payload with { Instruction = TextRequest.Value });
        
        await bot.SendTextMessageAsync(TextRequest.GetChatId(), "Готово",
            replyMarkup: new ReplyKeyboardMarkup
            ([
                ["Сохранить"]
            ]));

        recipeContext.NextState();
    }


    private async Task AddIngredient(TextRequest request, ITelegramBotClient bot,
        IDetailContext<RecipePayload, AddingRecipeStateContext> context)
    {
        var text = request.Value;
        if (request.Value == "Закончить")
        {
            await bot.SendTextMessageAsync(request.GetChatId(), "Теперь давай инструкцию");
            context.NextState();
            return;
        }

        AddIngredient(context, text);

        context.SaveChanges();
        await bot.SendTextMessageAsync(request.GetChatId(), "Добавить еще",
            replyMarkup: new ReplyKeyboardMarkup
            ([
                ["Закончить"]
            ]));
    }

    private static void AddIngredient(IDetailContext<RecipePayload, AddingRecipeStateContext> context, string text)
    { // пока есть косяк с инкасуляцией. мы меняет payload здесь, но не меняем json. сейчас это решает SaveChanges.
        context.Payload.Ingredients.TryAdd(text, new IngredientDetail(0, "штук"));

        var ingredient = context.Payload.Ingredients[text];
        ingredient = ingredient with { Count = ingredient.Count + 1 };

        context.Payload.Ingredients[text] = ingredient;
    }

    private void Error(TextRequest request, ChatContext context, ITelegramBotClient telegramBotClient)
    {
        telegramBotClient.SendTextMessageAsync(request.GetChatId(), "Я еще ничего не знаю");
    }


    private async Task SetName(TextRequest request, ITelegramBotClient botClient,
        IDetailContext<RecipePayload, AddingRecipeStateContext> context)
    {
        var payload = new RecipePayload
        {
            nameRecipe = request.Value
        };

        context.PostPayload(payload)
            .SaveChanges()
            .NextState();

        
        await botClient.SendTextMessageAsync(request.GetChatId(), "Какие ингредиенты");
    }
}
