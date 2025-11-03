using EasyTgBot.Abstract;
using Microsoft.Extensions.DependencyInjection;

namespace EasyTgBot.BuilderContext;

public class BuilderContextFlow<TState> where TState : struct, Enum
{
    private readonly IServiceCollection _collection;
    private readonly RangeFlowComponents<TState> _rangeFreeFlowComponent;
    internal readonly List<StateEvent> Steps = [];
    private readonly List<FlowNode<TState>> _nodes = new();

    internal BuilderContextFlow(RangeFlowComponents<TState> rangeFreeFlowComponent, IServiceCollection collection,
        bool isSubtask,
        List<StateEvent>? steps = null)
    {
        _collection = collection;
        _rangeFreeFlowComponent = rangeFreeFlowComponent;
        Steps = steps ?? [];
    }


    public BuilderContextFlow<TState> AddHandler<TContextHandler>(Action<BuilderContextFlow<TState>>? action = null)
        where TContextHandler : class, IContextHandler
    {
        if (_rangeFreeFlowComponent.Empty) throw new ArgumentException("capacity for handler is exhausted");
        var start = _rangeFreeFlowComponent.FreeState;
        _rangeFreeFlowComponent.Next();
        
        var node = new HandlerNode<TState>
        {
            State = start,
            HandlerType = typeof(TContextHandler)
        };

        if (action != null)
        {
            var subTaskBuilder = new BuilderContextFlow<TState>(_rangeFreeFlowComponent, _collection, true, Steps);

            action(subTaskBuilder);
            node.SubTasks.AddRange(subTaskBuilder._nodes);
        }

        _nodes.Add(node);
        _rangeFreeFlowComponent.PrevHandler = start;
        return this;
    }


    public BuilderContextFlow<TState> AddSwitch<TContextHandler>(
        params Action<BuilderContextFlowSwitch<TState>>[] action)
        where TContextHandler : class, IContextHandler
    {
        throw new NotImplementedException();
    }

    public void Build()
    {
        var serviceVisitor = new ServiceRegistrationVisitor<TState>(_collection);
        foreach (var node in _nodes)
            node.Accept(serviceVisitor);

        var eventVisitor = new StateEventGeneratorVisitor<TState>();
        foreach (var node in _nodes)
            node.Accept(eventVisitor);

        Steps.Clear();
        Steps.AddRange(eventVisitor.Events);
    }
}

public class BuilderContextFlowSwitch<TState> where TState : struct, Enum
{
    private readonly IServiceCollection _collection;
    private readonly RangeFlowComponents<TState> _rangeFreeFlowComponent;

    internal BuilderContextFlowSwitch(RangeFlowComponents<TState> rangeFreeFlowComponent, IServiceCollection collection,
        List<StateEvent> steps, string switchPosition)
    {
        _collection = collection;
        _rangeFreeFlowComponent = rangeFreeFlowComponent;
        Steps = steps;
    }

    public List<StateEvent> Steps { get; set; }
}