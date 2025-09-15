using CookingBot.Application.Commands.AddRecipe.Flow.ContextHandlers;
using CookingBot.Application.Flows.AddRecipe.InContexts;
using EasyTgBot;
using EasyTgBot.Abstract;
using EasyTgBot.Entity;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CookingBot.Application.Flows.AddRecipe;

public class AddRecipe(IChatContextRepository chatContextRepository)
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
            context.State = (int)AddingRecipeContext.AddingName;

            await chatContextRepository.Upsert(context);
            await bot.SendTextMessageAsync(chatId, $"Дай название рецепту");
        }
        else
        {
            await bot.SendTextMessageAsync(chatId, "Ты находишься где-то не там");
        }
    }
}