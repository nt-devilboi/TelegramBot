using EasyTgBot.Abstract;
using Microsoft.Extensions.DependencyInjection;

namespace EasyTgBot.BuilderContext;

public class BuilderContextFlow<TState> where TState : struct, Enum
{
    private readonly IServiceCollection _collection;
    private readonly RangeFlowComponents<TState> _rangeFreeFlowComponent;
    internal readonly List<StateEvent> Steps = [];
    private readonly TState[] _state;

    internal BuilderContextFlow(IServiceCollection collection, RangeFlowComponents<TState> rangeFreeFlowComponent)
    {
        _collection = collection;
        _rangeFreeFlowComponent = rangeFreeFlowComponent;
    }


    public BuilderContextFlow<TState> AddHandler<TContextHandler>(Action<BuilderSubFlowContext<TState>>? action = null)
        where TContextHandler : class, IContextHandler
    {
        if (_rangeFreeFlowComponent.Empty) throw new ArgumentException("capacity for handler is exhausted");


        _collection.AddScoped<TContextHandler>();

        var start = _rangeFreeFlowComponent.FreeState;

        if (!_rangeFreeFlowComponent.IsStart)
            Steps.Add(new StateEvent(Trigger.UserWantToContinue, _rangeFreeFlowComponent.PrevHandler, start));

        _collection.AddScoped<IHandlerInfo>(x =>
            new IHandlerInfo((IContextHandler)x.GetService(typeof(TContextHandler)), start.ToString()));

        _rangeFreeFlowComponent.Next();

        if (action != null)
        {
            var subTaskBuilder = new BuilderSubFlowContext<TState>(Steps, _rangeFreeFlowComponent, _collection);
            action(subTaskBuilder);
            var end = _rangeFreeFlowComponent.PrevState;
            Steps.Add(new StateEvent(Trigger.UserCompletedAllSubTask, end, start));
            for (var i = (int)(object)start + 1; i <= (int)(object)end; i++)
            {
                Steps.Add(new StateEvent(Trigger.UserGoToSubTask, start, (TState)(object)i,
                    ((TState)(object)i).ToString()));
            }
        }

        _rangeFreeFlowComponent.PrevHandler = start;
        return this;
    }
}