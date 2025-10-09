using EasyTgBot.Entity;
using Newtonsoft.Json;
using Stateless;

namespace EasyTgBot.Abstract;

public class DetailContext<TPayload, TState>
    where TState : struct, Enum
    where TPayload : BasePayload
{
    private TPayload? _payload;
    public readonly IStateMachine<TState> State;
    private readonly ChatContext _chatContext;

    internal DetailContext(ChatContext chatContext, IServiceRegistryFlow registryFlow)
    {
        var stateMachine = new StateMachine<TState, Trigger>(() => Enum.Parse<TState>(chatContext.State),
            x => chatContext.State = x.ToString());

        State = registryFlow.Wraps(stateMachine);
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

    public DetailContext<TPayload, TState> Reset()
    {
        _chatContext.ToUserAccount();
        _chatContext.Payload = null;
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