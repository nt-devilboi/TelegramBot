using EasyTgBot.Abstract;
using Microsoft.Extensions.DependencyInjection;

namespace EasyTgBot.BuilderContext;

public class BuilderSubFlowContext<TState>(
    List<StateEvent> steps,
    RangeFlowComponents<TState> rangeFlowComponents,
    IServiceCollection collection) where TState : struct, Enum
{
    public BuilderSubFlowContext<TState> AddSubHandler<TContextHandler>() where TContextHandler : class, IContextHandler
    {
        if (rangeFlowComponents.Empty)
        {
            throw new ArgumentException("capacity for handler is exhausted");
        }

        collection.AddScoped<TContextHandler>();

        var cur = rangeFlowComponents.FreeState;
        var prev = rangeFlowComponents.PrevState;
        if (!rangeFlowComponents.IsStart)
        {
            steps.Add(new StateEvent(Trigger.UserCompletedSubTask, prev, cur));
            rangeFlowComponents.Next();
        }

        collection.AddScoped<IHandlerInfo>(x =>
            new IHandlerInfo((IContextHandler)x.GetService(typeof(TContextHandler)), cur.ToString()));

        return this;
    }
}