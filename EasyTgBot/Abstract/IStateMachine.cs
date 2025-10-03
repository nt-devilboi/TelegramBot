using Stateless;

namespace EasyTgBot.Abstract;

public interface IStateMachine<in TState>
{
    void Continue();
    void GoTo(TState state);
}

public class StateMachine<TState>(StateMachine<TState, Trigger> stateMachine) : IStateMachine<TState>
{
    private readonly StateMachine<TState, Trigger>.TriggerWithParameters<string> _goToSubTask =
        new(Trigger.UserGoToSubTask);

    public void Continue()
    {
        stateMachine.Fire(Trigger.UserWantToContinue);
    }

    public void GoTo(TState state)
    {
        stateMachine.Fire(_goToSubTask, state.ToString());
    }

    public static implicit operator StateMachine<TState>(StateMachine<TState, Trigger> stateMachine) =>
        new(stateMachine);
}