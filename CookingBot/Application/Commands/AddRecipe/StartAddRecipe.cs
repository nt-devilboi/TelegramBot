using CookingBot.Commands.AddRecipe.Flow;
using EasyTgBot;
using EasyTgBot.Abstract;
using EasyTgBot.Entity;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CookingBot.Application.Commands.AddRecipe;

public class StartAddRecipe(IChatContextRepository chatContextRepository)
    : ICommand
{
    public string Trigger { get; } = "Добавить рецепт";
    public string Desc { get; }

    public async Task Execute(Update request, ITelegramBotClient bot, ChatContext context)
    {
        var chatId = request.Message.Chat.Id;

        if (context.State == 0)
        {
            await bot.SendTextMessageAsync(chatId, "Сначала нужно авторизоваться");
            return;
        }

        if (context.InUserAccount())
        {
            context.State = (int)AddingRecipeStateContext.AddingName;

            await chatContextRepository.Upsert(context);
            await bot.SendTextMessageAsync(chatId, $"Дай название рецепту");
        }
        else
        {
            await bot.SendTextMessageAsync(chatId, "Ты находишься где-то не там");
        }
    }
}