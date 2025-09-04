using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using EasyTgBot.Entity;
using Newtonsoft.Json;

namespace EasyTgBot.Abstract;

public abstract class IDetailContext<TPayload, TState> where TState : Enum
{
    private TPayload? _payload;
    private readonly ITransactionService _transactionService;
    public TPayload Payload
    {
        get
        {
            if (_payload == null)
                throw new NullReferenceException($"Payload in {typeof(TPayload)} is null");

            return _payload;
        }
        protected set => _payload = value;
    }

    public IDetailContext<TPayload, TState> SaveChanges()
    {
        ChatContext.Payload = JsonConvert.SerializeObject(Payload);
        WasChanged = true;

        return this;
    }
    protected IDetailContext(ChatContext chatContext, TPayload? payload,
        ITransactionService transactionService)
    {
        ChatContext = chatContext;
        Payload = payload;
        _transactionService = transactionService;
    }
    public IDetailContext<TPayload, TState> NextState()
    {
        _transactionService.NextState(ChatContext);
        WasChanged = true;

        return this;
    }
    public IDetailContext<TPayload, TState> PostPayload(TPayload payload)
    {
        Payload = payload;
        ChatContext.Payload = JsonConvert.SerializeObject(payload);

        return this;
    }

    public TState State => (TState)(object)ChatContext.State;
    public bool WasChanged { get; private set; }

    private ChatContext ChatContext { get; }

    private IDetailContext(ChatContext chatContext)
    {
        ChatContext = chatContext;
    }
}