using System.Diagnostics.CodeAnalysis;
using EasyTgBot.Abstract;
using EasyTgBot.Entity;
using Newtonsoft.Json;
using JsonException = System.Text.Json.JsonException;

namespace CookingBot.Commands.AddRecipe.Flow;

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

    //todo: как будто лучше выкинуть это в абстрактную фабрику.
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

public class TransactionService : ITransactionService
{
    private readonly Dictionary<AddingRecipeStateContext, AddingRecipeStateContext> NextStepFrom = new()
    {
        {
            AddingRecipeStateContext.AddingName, AddingRecipeStateContext.AddingIngredient
        },
        {
            AddingRecipeStateContext.AddingIngredient, AddingRecipeStateContext.AddingInstruction
        },
        {
            AddingRecipeStateContext.AddingInstruction, AddingRecipeStateContext.SaveRecipe
        },
        {
            AddingRecipeStateContext.SaveRecipe, AddingRecipeStateContext.ReturnToMenu
        }
    };

    private readonly Dictionary<AddingRecipeStateContext, AddingRecipeStateContext> CanMove = new()
    {
        {
            AddingRecipeStateContext.AddingName, AddingRecipeStateContext.AddingIngredient
        }
    };

    public void NextState(ChatContext context)
    {
        context.State = (int)NextStepFrom[(AddingRecipeStateContext)context.State];
    }

    public void NextState(ChatContext context, AddingRecipeStateContext addingRecipeStateContext)
    {
        if (CanMove[(AddingRecipeStateContext)context.State] == addingRecipeStateContext)
            context.State = (int)NextStepFrom[(AddingRecipeStateContext)context.State];
    }
}

public enum AddingRecipeStateContext
{
    ReturnToMenu = 1,
    AddingName = 10000,
    AddingIngredient,
    AddingInstruction,
    SaveRecipe
}