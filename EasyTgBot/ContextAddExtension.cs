using EasyTgBot.Abstract;
using EasyTgBot.BuilderContext;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace EasyTgBot;

public static class ContextAddExtension
{
    public static void AddContext<TEnum>(this IServiceCollection serviceCollection, string trigger,
        Action<BuilderContextFlow<TEnum>> builderFunc, IServiceRegistryFlow registryFlow) where TEnum : struct, Enum
    {
        var enums = Enum.GetValues<TEnum>();
        var builder = new BuilderContextFlow<TEnum>(serviceCollection, new RangeFlowComponents<TEnum>(enums));

        serviceCollection.AddScoped<Command>(_ => new Router<TEnum>(trigger));
        var stateType = typeof(TEnum);
        var duplicate = serviceCollection.Any(SameContext<TEnum>(trigger));
        if (duplicate)
            throw new InvalidOperationException(
                $"Trigger descriptor for state '{stateType.FullName}' already registered.");
        
        serviceCollection.AddSingleton<IRouterTriggerDescriptor>(new RouterTriggerDescriptor(stateType, trigger));
        builderFunc(builder);

        registryFlow.AddFlow<TEnum>(builder.Steps);
    }

    private static Func<ServiceDescriptor, bool> SameContext<TEnum>(string trigger) where TEnum : struct, Enum
    {
        return d => d.ServiceType == typeof(IRouterTriggerDescriptor)
                    && d.ImplementationInstance is RouterTriggerDescriptor r
                    && (r.StateType == typeof(Enum) || r.Trigger == trigger);
    }

    public static IServiceCollection AddTriggerProvider(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<ITriggerProvider, TriggerProvider>();
        return serviceCollection;
    }
}

public class RangeFlowComponents<TState>(TState[] state)
{
    private int _pointer;
    public TState FreeState => state[_pointer];
    public TState PrevState => state[_pointer - 1];
    private readonly TState _start = state[0];

    public bool IsStart => _pointer == 0;
    public TState PrevHandler { get; set; } = state[0];
    public bool Empty => _pointer >= state.Length;

    public void Next() => _pointer++;
}

internal record IHandlerInfo(IContextHandler ContextHandler, string number);