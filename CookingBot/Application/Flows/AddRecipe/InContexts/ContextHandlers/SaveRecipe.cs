using CookingBot.Application.Commands;
using CookingBot.Application.Interfaces;
using CookingBot.Domain.Entity;
using CookingBot.Domain.Payloads;
using EasyTgBot;
using EasyTgBot.Abstract;
using EasyTgBot.Entity;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace CookingBot.Application.Flows.AddRecipe.InContexts.ContextHandlers;

public class SaveRecipe(
    IRecipeRepository recipeRepository,
    ITelegramBotClient botClient)
    : ContextHandler<RecipePayload, AddingRecipeContext>
{
    protected override async Task Handle(Update update,
        DetailContext<RecipePayload, AddingRecipeContext> context)
    {
        var request = update.AsRequestWithText();
        if (request.Value != "Сохранить")
        {
            await botClient.SendTextMessageAsync(request.GetChatId(), "Я тебя не понимаю");
            return;
        }

        if (!context.TryGetPayload(out var payload)) return;

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


        await botClient.SendTextMessageAsync(request.GetChatId(), Phrase.Recipe.Save);
        await botClient.SendTextMessageAsync(request.GetChatId(), "Теперь можешь выполнить эти команды",
            replyMarkup: new ReplyKeyboardMarkup([
                CheckMyRecipe.staticTrigger,
                WantToCook.WantToCook.StaticTrigger
            ]));
        context.Reset();
    }
}