using System.ComponentModel.DataAnnotations;
using EasyTgBot.Entity;

namespace EasyTgBot.Abstract;

public abstract class TransactionService<TState> where TState : Enum
{
    [Required] protected abstract Dictionary<TState, TState> NextStepFrom { get; }
    [Required] protected abstract Dictionary<TState, TState> CanMove { get; }

    public void NextState(ChatContext context)
    {
        if (NextStepFrom.TryGetValue((TState)(object)context.State, out var state))
        {
            context.State = (int)(object)state;
        }
        else
        {
            ToUserAccount(context);
        }
    }

    public void ToUserAccount(ChatContext context)
    {
        context.State = (int)BaseContextState.UserAccount;
    }
}