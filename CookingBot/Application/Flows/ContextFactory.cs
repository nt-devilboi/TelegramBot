using EasyTgBot.Abstract;
using EasyTgBot.Entity;
using Newtonsoft.Json;
using JsonException = System.Text.Json.JsonException;

namespace CookingBot.Application.Flow;

public class ContextFactory<TPayload, TService, TState>
    where TState : Enum
    where TService : TransactionService<TState>, new()
    where TPayload : BasePayload
{
    public static DetailContext<TPayload, TState> Create(ChatContext context)
    {
        if (string.IsNullOrEmpty(context.Payload))
            return new DetailContext<TPayload, TState>(context, default,
                new TService()); //todo: у каждого будет свой transactionService

        var payload = JsonConvert.DeserializeObject<TPayload>(context.Payload);
        if (payload == null) throw new JsonException($"payload: {context.Payload} not {typeof(TPayload)}");

        return new DetailContext<TPayload, TState>(context, payload,
            new TService());
    }

    protected virtual TPayload? DeserializePayload(string? payload)
    {
        if (string.IsNullOrEmpty(payload)) return default;

        var result = JsonConvert.DeserializeObject<TPayload>(payload);

        if (result == null)
            throw new JsonException($"payload: {payload} not {typeof(TPayload)}");

        return result;
    }
}