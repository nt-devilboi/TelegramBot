using System.Diagnostics.CodeAnalysis;
using EasyTgBot.Abstract;
using EasyTgBot.Entity;
using Newtonsoft.Json;
using JsonException = System.Text.Json.JsonException;

namespace CookingBot.Application.Commands.AddRecipe.Flow;

public class RecipeContext<TPayload, TState> : IDetailContext<TPayload, TState> where TState : Enum
{
    private RecipeContext(ChatContext chatContext, TPayload? payload, ITransactionService transactionService) :
        base(chatContext, payload, transactionService)
    {
    }

    
    //todo: как будто лучше выкинуть это в абстрактную фабрику. и тем самым одним extensions можно будет добавлять все элементы flow. Add(typeof(FlodName. или пока оставить так.
    public static IDetailContext<TPayload, TState> Create(ChatContext context)
    {
        if (string.IsNullOrEmpty(context.Payload))
            return new RecipeContext<TPayload, TState>(context, default,
                new TransactionService()); //todo: у каждого будет свой transactionService

        var payload = JsonConvert.DeserializeObject<TPayload>(context.Payload);
        if (payload == null) throw new JsonException($"payload: {context.Payload} not {typeof(TPayload)}");

        return new RecipeContext<TPayload, TState>(context, payload, new TransactionService());
    }

    
}