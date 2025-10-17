using CookingBot.Application.Commands;
using CookingBot.Application.Flows.WantToCook.InContexts;
using EasyTgBot.Abstract;

namespace CookingBot.Application.Flows.WantToCook;

public class WantToCook 
{

    public static readonly string StaticTrigger = "Хочу приготовить";
    public Priority Priority { get; } = Priority.Command;
}