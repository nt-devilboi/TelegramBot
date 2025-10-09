using EasyTgBot.Abstract;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace CookingBot.Application.Flows.Friends;

public class Friends(ITelegramBotClient botClient) : ContextHandler<BasePayload, FriendsContext>
{
    private (string AddFriend, string CheckOutFriendCooking) buttoms = ("Добавить друга",
        "Посмотреть, что готовил друг");

    protected override async Task Handle(Update update, DetailContext<BasePayload, FriendsContext> context)
    {
        if (update.Message.Text == buttoms.AddFriend)
        {
            context.State.GoTo(FriendsContext.AddFriend);
            
        }
    }

    protected override async Task Enter(DetailContext<BasePayload, FriendsContext> context)
    {
        await botClient.SendTextMessageAsync(context.ChatId, "Что хочешь сделать?",
            replyMarkup: new ReplyKeyboardMarkup([buttoms.AddFriend, buttoms.CheckOutFriendCooking]));
    }
}