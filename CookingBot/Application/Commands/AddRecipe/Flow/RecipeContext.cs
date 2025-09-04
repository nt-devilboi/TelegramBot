using System.Diagnostics.CodeAnalysis;
using EasyTgBot.Abstract;
using EasyTgBot.Entity;
using Newtonsoft.Json;
using JsonException = System.Text.Json.JsonException;

namespace CookingBot.Application.Commands.AddRecipe.Flow;

public class RecipeContext<TPayload, TState> : IDetailContext<TPayload, TState> where TState : Enum
{
    private readonly ITransactionService _transactionService;
    public bool WasChanged { get; private set; }

    public TState State => (TState)(object)ChatContext.State;

    [field: AllowNull, MaybeNull]
    public TPayload Payload
    {
        get
        {
            if (field == null) throw new NullReferenceException($"Payload in {typeof(TPayload)} is null");

            return field;
        }
        private set;
    }

    private ChatContext ChatContext { get; }

    private RecipeContext(ChatContext chatContext, TPayload? payload,
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

    //todo: как будто лучше выкинуть это в абстрактную фабрику. и тем самым одним extensions можно будет добавлять все элементы flow. Add(typeof(FlodName..
    public static IDetailContext<TPayload, TState> Create(ChatContext context)
    {
        if (string.IsNullOrEmpty(context.Payload))
            return new RecipeContext<TPayload, TState>(context, default,
                new TransactionService()); //todo: у каждого будет свой transactionService

        var payload = JsonConvert.DeserializeObject<TPayload>(context.Payload);
        if (payload == null) throw new JsonException($"payload: {context.Payload} not {typeof(TPayload)}");

        return new RecipeContext<TPayload, TState>(context, payload, new TransactionService());
    }

    public IDetailContext<TPayload, TState> SaveChanges()
    {
        ChatContext.Payload = JsonConvert.SerializeObject(Payload);
        WasChanged = true;

        return this;
    }
}