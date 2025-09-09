using EasyTgBot.Entity;
using Newtonsoft.Json;

namespace EasyTgBot.Abstract;

public class DetailContext<TPayload, TState>(
    ChatContext chatContext,
    TPayload? payload,
    TransactionService<TState> transactionService)
    where TState : Enum
    where TPayload : BasePayload //понятно, что здесь не очень нужен абстрактный класс, но я еще не понимаю насколько можно сделать одну такую сущность гибой. иначе говоря, можно сделать один класс и всё
{
    private TPayload? _payload = payload;

    public bool TryGetPayload(out TPayload payload) // можно заюзать паттерн IDisplosable. но думаю это нужно еще класс вроде писать. 
    {
        if (_payload != null)
        {
            payload = _payload with { };
            return true;
        }

        payload = null;
        return false;
    }


    public DetailContext<TPayload, TState> NextState()
    {
        transactionService.NextState(ChatContext);
        WasChanged = true;

        return this;
    }

    public DetailContext<TPayload, TState> ToUserAccount()
    {
        transactionService.ToUserAccount(ChatContext);
        WasChanged = true;
        return this;
    }

    public DetailContext<TPayload, TState> UpdatePayload(TPayload payload)
    {
        _payload = payload;
        ChatContext.Payload = JsonConvert.SerializeObject(_payload);
        WasChanged = true;

        return this;
    }

    public TState State => (TState)(object)ChatContext.State;
    public bool WasChanged { get; private set; }

    private ChatContext ChatContext { get; } = chatContext;
}

public abstract record BasePayload;