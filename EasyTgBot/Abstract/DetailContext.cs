using EasyTgBot.Entity;
using Newtonsoft.Json;
using Stateless;

namespace EasyTgBot.Abstract;

public class DetailContext<TPayload, TState>
    where TState : struct, Enum
    where TPayload : BasePayload
{
    private TPayload? _payload;
    private readonly StateMachine<TState, Trigger> _stateMachine;
    private readonly ChatContext _chatContext;

    internal DetailContext(ChatContext chatContext, IServiceRegistryFlow registryFlow)
    {
        _stateMachine = new StateMachine<TState, Trigger>(() => (TState)(object)chatContext.State,
            x => chatContext.State = (int)(object)x);

        registryFlow.Wraps(_stateMachine);
        _chatContext = chatContext;
        _payload = JsonConvert.DeserializeObject<TPayload>(_chatContext.Payload ?? string.Empty);
    }

    public bool TryGetPayload(out TPayload payload)
    {
        if (_payload != null)
        {
            payload = _payload;
            return true;
        }

        payload = null;
        return false;
    }

    public long ChatId => _chatContext.ChatId;

    public DetailContext<TPayload, TState> NextState()
    {
        _stateMachine.Fire(Trigger.UserWantToContinue);
        return this;
    }

    public void GoTo(TState state)
    {
        var triggerWithParameters =
            new StateMachine<TState, Trigger>.TriggerWithParameters<string>(Trigger.UserGoToSubTask);
        _stateMachine.Fire(triggerWithParameters, state.ToString());
    }

    public DetailContext<TPayload, TState> ToUserAccount()
    {
        _chatContext.ToUserAccount();
        return this;
    }

    public DetailContext<TPayload, TState> UpdatePayload(TPayload payload)
    {
        _payload = payload;
        _chatContext.Payload = JsonConvert.SerializeObject(_payload);

        return this;
    }
}

public abstract record BasePayload;