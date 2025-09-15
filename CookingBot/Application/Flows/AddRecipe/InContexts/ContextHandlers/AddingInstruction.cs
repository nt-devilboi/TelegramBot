using CookingBot.Application.Flow;
using CookingBot.Application.Flows.AddRecipe.InContexts;
using CookingBot.Domain.Payloads;
using EasyTgBot;
using EasyTgBot.Abstract;
using EasyTgBot.Entity;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace CookingBot.Application.Commands.AddRecipe.Flow.ContextHandlers;

public class AddingInstruction(IChatContextRepository chatContextRepository) : IContextHander
{
    public async Task Handle(Update update, ITelegramBotClient bot, ChatContext context)
    {
        var recipeContext =
            ContextFactory<RecipePayload, TransactionServiceRecipe, AddingRecipeContext>.Create(context);
        var request = update.AsRequestWithText();
        if (recipeContext.TryGetPayload(out var payload))
        {
            payload = payload with { Instruction = request.Value };
            await bot.SendTextMessageAsync(request.GetChatId(), "Готово",
                replyMarkup: new ReplyKeyboardMarkup
                ([
                    ["Сохранить"]
                ]));
            recipeContext.UpdatePayload(payload)
                .NextState();

            await chatContextRepository.Upsert(context);
        }
    }
}