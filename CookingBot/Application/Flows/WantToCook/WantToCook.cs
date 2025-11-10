using BotOrchestriX.Abstract;

namespace CookingBot.Application.Flows.WantToCook;

public class WantToCook
{
    public static readonly string StaticTrigger = "Хочу приготовить";
    public Priority Priority { get; } = Priority.Command;
}