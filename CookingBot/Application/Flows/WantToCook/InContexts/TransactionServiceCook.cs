using EasyTgBot.Abstract;

namespace CookingBot.Application.Flows.WantToCook.InContexts;

public class TransactionServiceCook : TransactionService<CookContext>
{
    protected override Dictionary<CookContext, CookContext> NextStepFrom { get; } = new()
    {
        { CookContext.ChoosingDish, CookContext.Cooking }
    };

    protected override Dictionary<CookContext, CookContext> CanMove { get; }
}