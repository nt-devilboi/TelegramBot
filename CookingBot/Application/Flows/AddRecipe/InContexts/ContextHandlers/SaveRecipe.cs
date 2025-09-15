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

public class SaveRecipe(IRecipeRepository recipeRepository, IChatContextRepository chatContextRepository)
    : IContextHander
{
    public async Task Handle(Update update, ITelegramBotClient bot, ChatContext context)
    {
        var recipeContext =
            ContextFactory<RecipePayload, TransactionServiceRecipe, AddingRecipeContext>.Create(context);
        var request = update.AsRequestWithText();
        if (request.Value != "Сохранить")
        {
            await bot.SendTextMessageAsync(request.GetChatId(), "Я тебя не понимаю");
            return;
        }

        if (!recipeContext.TryGetPayload(out var payload)) return;

        var recipe =
            new
                Recipe() // если вдруг будет появляется возможность создать этот класс еще где-то, то тогда наверное это имеет смысл перенети в конструктор или в extension, так как уже будет код повторяться, а пока ожидается, что это будет только здесь.
                {
                    nameRecipe = payload.nameRecipe.ToLower(),
                    Ingredients = payload.Ingredients,
                    Instruction = payload.Instruction,
                    Id = Guid.NewGuid(),
                    ChatId = request.GetChatId()
                };

        await recipeRepository.Upsert(recipe);


        await bot.SendTextMessageAsync(request.GetChatId(), Phrase.Recipe.Save);
        recipeContext.ToUserAccount();
        await chatContextRepository.Upsert(context);
    }
}