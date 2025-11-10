using BotOrchestriX.Abstract;
using CookingBot.Infrastructure;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CookingBot.Application.Flows.Friends.InContext;

/*public class AddFriend(ITelegramBotClient botClient, IChatRepository<ChatWithAuth> chatRepository, IFriendRepository friendRepository)
    : ContextHandler<BasePayload, FriendsContext>
{
    protected override async Task Handle(Update update, DetailContext<BasePayload, FriendsContext> context)
    {
        if (long.TryParse(update.Message.Text, out var friendId))
        {
            if (await chatRepository.Get(friendId) == null)
            {
                await botClient.SendTextMessageAsync(context.ChatId, "Такого пользователя нету");
                return;
            }

            await friendRepository.Add(new Friend { UserId = context.ChatId, FriendId = friendId });

            await botClient.SendTextMessageAsync(context.ChatId, "Добавил друга");
        }
        else
        {
            await botClient.SendTextMessageAsync(context.ChatId, "Должно быть число");
        }
    }

    protected override async Task Enter(DetailContext<BasePayload, FriendsContext> context)
    {
        await botClient.SendTextMessageAsync(context.ChatId, "Введи id пользователя");
        await botClient.SendTextMessageAsync(context.ChatId, $"Либо дай свой: {context.ChatId}");
    }
}*/