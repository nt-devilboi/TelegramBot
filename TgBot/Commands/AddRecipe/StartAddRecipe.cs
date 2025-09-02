using EasyTgBot.Entity;
using EasyTgBot.Restored.Abstract;
using Telegram.Bot;
using TgBot.Commands.AddRecipe.Flow;
using TgBot.Domain.Entity;

namespace TgBot.Commands;

public class StartAddRecipe(
    IChatRepository chatRepository,
    IChatContextRepository chatContextRepository)
    : ICommand
{
    private readonly IChatRepository _chatRepository = chatRepository;
    public string Name { get; } = "Добавить рецепт";
    public string Desc { get; }

    public async Task Execute(ITgRequest? request, ITelegramBotClient bot, ChatContext context)
    {
        var chatId = request.Message.Chat.Id;


        if (context.State == 0)
        {
            await bot.SendTextMessageAsync(chatId, "Сначала нужно авторизоваться");
            return;
        }

        if (context.State == (int)ContextState.Menu)
        {
            context.State = RecipeContext.AddingName;

            await chatContextRepository.Upsert(context);
            await bot.SendTextMessageAsync(chatId, $"{context.State}: {context.Payload}");
        }
        else
        {
            await bot.SendTextMessageAsync(chatId, "Ты находишься где-то не там");
        }
    }
}