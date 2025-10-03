using Stateless;

namespace EasyTgBot.Abstract;

public interface IServiceRegistryFlow
{
    void AddFlow<TState>(List<StateEvent> stateEvents);

    IStateMachine<TState> Wraps<TState>(StateMachine<TState, Trigger> stateMachine) where TState : struct, Enum;
}

public class ServiceRegistryFlow : IServiceRegistryFlow // мб в будущем сделать internal
{
    private readonly Dictionary<Type, List<StateEvent>> Flows = new();

    public void AddFlow<TState>(List<StateEvent> stateEvents)
    {
        Flows.Add(typeof(TState), stateEvents);
    }

    public IStateMachine<TState> Wraps<TState>(StateMachine<TState, Trigger> stateMachine)
        where TState : struct, Enum

    {
        var approveTrigger = stateMachine.SetTriggerParameters<string>(Trigger.UserGoToSubTask);
        foreach (var stateEvent in Flows[typeof(TState)])
        {
            var stateConfiguration = stateMachine.Configure((TState)(object)stateEvent.Source);
            if (stateEvent.Trigger == Trigger.UserCompletedSubTask)
            {
                stateMachine.Configure((TState)(object)stateEvent.Dest).SubstateOf((TState)(object)stateEvent.Source);
            }

            if (stateEvent.Trigger == Trigger.UserGoToSubTask)
            {
                stateConfiguration.PermitIf(approveTrigger, (TState)(object)stateEvent.Dest,
                    x => stateEvent.NameHandler == x);
                continue;
            }

            stateConfiguration.Permit(stateEvent.Trigger, (TState)(object)stateEvent.Dest);
        }


        return new StateMachine<TState>(stateMachine);
    }
}

public record StateEvent(Trigger Trigger, int Source, int Dest, string? NameHandler = null);