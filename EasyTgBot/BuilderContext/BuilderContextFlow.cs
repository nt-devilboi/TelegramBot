using EasyTgBot.Abstract;
using Microsoft.Extensions.DependencyInjection;

namespace EasyTgBot.BuilderContext;

public class BuilderContextFlow<TState>
{
    private readonly IServiceCollection _collection;
    private readonly RangeFlowComponents _rangeFreeFlowComponent;
    internal readonly List<StateEvent> Steps = [];

    internal BuilderContextFlow(IServiceCollection collection, RangeFlowComponents rangeFreeFlowComponent)
    {
        _collection = collection;
        _rangeFreeFlowComponent = rangeFreeFlowComponent;
    }


    public BuilderContextFlow<TState> AddHandler<TContextHandler>(Action<BuilderSubFlowContext<TState>>? action = null)
        where TContextHandler : class, IContextHandler
    {
        if (_rangeFreeFlowComponent.Empty) throw new ArgumentException("capacity for handler is exhausted");


        _collection.AddScoped<TContextHandler>();

        var start = _rangeFreeFlowComponent.GetIdFreeComponent;

        if (!_rangeFreeFlowComponent.IsStart)
            Steps.Add(new StateEvent(Trigger.UserWantToContinue, _rangeFreeFlowComponent.PrevHandler, start));

        _collection.AddScoped<IHandlerInfo>(x =>
            new IHandlerInfo((IContextHandler)x.GetService(typeof(TContextHandler)), start.ToString()));

        _rangeFreeFlowComponent.Next();

        if (action != null)
        {
            var subTaskBuilder = new BuilderSubFlowContext<TState>(Steps, _rangeFreeFlowComponent, _collection);
            action(subTaskBuilder);
            var end = _rangeFreeFlowComponent.GetIdFreeComponent - 1;
            Steps.Add(new StateEvent(Trigger.UserCompletedAllSubTask, end, start));
            for (var i = start + 1; i <= end; i++)
            {
                Steps.Add(new StateEvent(Trigger.UserGoToSubTask, start, i, ((TState)(object)i).ToString()));
            }
        }

        _rangeFreeFlowComponent.PrevHandler = start;
        return this;
    }
}