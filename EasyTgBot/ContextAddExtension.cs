using EasyTgBot.Abstract;
using EasyTgBot.BuilderContext;
using Microsoft.Extensions.DependencyInjection;
using Stateless;

namespace EasyTgBot;

public static class ContextAddExtension
{
    public static void AddContext<TEnum>(this IServiceCollection serviceCollection,
        Action<BuilderContextFlow<TEnum>> builderFunc, IServiceRegistryFlow registryFlow) where TEnum : struct, Enum
    {
        var enums = Enum.GetValues<TEnum>();
        var builder = new BuilderContextFlow<TEnum>(serviceCollection, new RangeFlowComponents<TEnum>(enums));

        builderFunc(builder);

        registryFlow.AddFlow<TEnum>(builder.Steps);
    }
}

public class RangeFlowComponents<TState>(TState[] state)
{
    private int _pointer;
    public TState FreeState => state[_pointer];
    public TState PrevState => state[_pointer - 1];
    private readonly TState _start = state[0];

    public bool IsStart => _pointer == 0;
    public TState PrevHandler { get; set; }
    public bool Empty => _pointer >= state.Length;

    public void Next() => _pointer++;
}

internal record IHandlerInfo(IContextHandler ContextHandler, string number);