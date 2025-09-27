using CookingBot.Domain.Payloads;
using EasyTgBot;
using EasyTgBot.Abstract;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace CookingBot.Application.Flows.AddRecipe.InContexts.ContextHandlers;

public class AddingInstruction(IContextRepository contextRepository, ITelegramBotClient botClient)
    : ContextHandler<RecipePayload, AddingRecipeContext>
{
    protected override async Task Handle(Update update,
        DetailContext<RecipePayload, AddingRecipeContext> context)
    {
        var request = update.AsRequestWithText();
        if (context.TryGetPayload(out var payload))
        {
            payload = payload with { Instruction = request.Value };
            context.UpdatePayload(payload)
                .NextState();

            await botClient.SendTextMessageAsync(request.GetChatId(), "Готово", replyMarkup: GetSaveButton());
        }
    }

    private static ReplyKeyboardMarkup GetSaveButton()
    {
        return new ReplyKeyboardMarkup
        ([
            ["Сохранить"]
        ]);
    }
}
// а норм ли что инфу про следующие иструкцию даёт прошошлая. не слишком ли появляетяс зависимость.