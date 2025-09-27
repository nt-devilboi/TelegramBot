using EasyTgBot.Abstract;
using EasyTgBot.Entity;

namespace EasyTgBot;

internal class ContextFactory(IServiceRegistryFlow flows) : IContextFactory
{
    public DetailContext<TPayload, TState> Create<TPayload, TState>(ChatContext context)
        where TPayload : BasePayload where TState : struct, Enum
    {
        return new DetailContext<TPayload, TState>(context, flows);
    }
}

internal interface IContextFactory
{
    public DetailContext<TPayload, TState> Create<TPayload, TState>(ChatContext context)
        where TState : struct, Enum where TPayload : BasePayload;
}