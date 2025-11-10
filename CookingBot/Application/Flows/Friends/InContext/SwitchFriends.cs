using BotOrchestriX.Abstract;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace CookingBot.Application.Flows.Friends.InContext;

public class SwitchFriends(ITelegramBotClient botClient)
    : ContextHandler<BasePayload, FriendsContext>
{
    private static readonly (string addFriend, string checkRecipes) Buttons = ("Добавить друга", "Посмотреть рецепты");

    protected override async Task Handle(Update update, DetailContext<BasePayload, FriendsContext> context)
    {
        if (update.Message?.Text == Buttons.addFriend)
        {
            context.State.GoTo(FriendsContext.AddFriend);
            return;
        }

        if (update.Message?.Text == Buttons.checkRecipes) context.State.GoTo(FriendsContext.CheckRecipe);
    }

    protected override async Task Enter(DetailContext<BasePayload, FriendsContext> context)
    {
        await botClient.SendTextMessageAsync(context.ChatId, "Что хочешь сделать?",
            replyMarkup: new ReplyKeyboardMarkup([Buttons.addFriend, Buttons.checkRecipes]));
    }
}