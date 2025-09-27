using EasyTgBot.Abstract;
using Microsoft.Extensions.DependencyInjection;

namespace EasyTgBot.BuilderContext;

public class BuilderSubFlowContext<TState>(
    List<StateEvent> steps,
    RangeFlowComponents rangeFlowComponents,
    IServiceCollection collection)
{
    public BuilderSubFlowContext<TState> AddSubHandler<TContextHandler>() where TContextHandler : class, IContextHandler
    {
        if (rangeFlowComponents.Empty)
        {
            throw new ArgumentException("capacity for handler is exhausted");
        }

        collection.AddScoped<TContextHandler>();

        var cur = rangeFlowComponents.GetIdFreeComponent;
        if (!rangeFlowComponents.IsStart)
        {
            steps.Add(new StateEvent(Trigger.UserCompletedSubTask, cur - 1, cur));
            rangeFlowComponents.Next();
        }

        collection.AddScoped<IHandlerInfo>(x =>
            new IHandlerInfo((IContextHandler)x.GetService(typeof(TContextHandler)), cur.ToString()));

        return this;
    }
}