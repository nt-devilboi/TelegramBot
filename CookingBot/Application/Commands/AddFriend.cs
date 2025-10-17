using EasyTgBot.Abstract;

namespace CookingBot.Application.Commands;

public class AddFriend
{
    public string Trigger { get; } = "Посмотреть друзей";
    public Priority Priority { get; }
}