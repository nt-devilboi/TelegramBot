using CookingBot.Application.Flows;
using EasyTgBot.Abstract;
using EasyTgBot.Entity;
using Telegram.Bot.Types;

namespace CookingBot.Application.Commands;

public class AddFriend : Router, ICommand
{
    public string Trigger { get; } = "Добавить друга";
    public string Desc { get; }
    public Priority Priority { get; }


    public async Task Execute(Update update, ChatContext context)
    {
        Route<FriendsContext>(context);
    }
}

public abstract class Router
{
    protected void Route<TEnum>(ChatContext context) where TEnum : struct, Enum
    {
        var firstState = Enum.GetValues<TEnum>()[0];

        context.State = firstState.ToString();
    }
}