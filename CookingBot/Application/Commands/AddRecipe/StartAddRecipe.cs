using CookingBot.Commands.AddRecipe.Flow;
using EasyTgBot.Abstract;
using EasyTgBot.Entity;
using Telegram.Bot;
using Telegram.Bot.Types;
using CookingBot.Domain.Entity;

namespace CookingBot.Commands;

public class StartAddRecipe(
    IChatRepository chatRepository,
    IChatContextRepository chatContextRepository)
    : ICommand
{
    private readonly IChatRepository _chatRepository = chatRepository;
    public string Name { get; } = "Добавить рецепт";
    public string Desc { get; }

    public async Task Execute(Update request, ITelegramBotClient bot, ChatContext context)
    {
        var chatId = request.Message.Chat.Id;

        if (context.State == 0)
        {
            await bot.SendTextMessageAsync(chatId, "Сначала нужно авторизоваться");
            return;
        }

        if (context.State == (int)ContextState.Menu)
        {
            context.State = (int)AddingRecipeStateContext.AddingName;

            await chatContextRepository.Upsert(context);
            await bot.SendTextMessageAsync(chatId, $"{context.State}: {context.Payload}");
        }
        else
        {
            await bot.SendTextMessageAsync(chatId, "Ты находишься где-то не там");
        }
    }
}