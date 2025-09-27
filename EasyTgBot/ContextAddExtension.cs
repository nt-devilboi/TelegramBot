using EasyTgBot.Abstract;
using Microsoft.Extensions.DependencyInjection;
using Stateless;

namespace EasyTgBot;

public static class ContextAddExtension
{
    public static void AddContext<TEnum>(this IServiceCollection serviceCollection,
        Action<BuilderContextFlow<TEnum>> builderFunc, IServiceRegistryFlow registryFlow) where TEnum : struct, Enum
    {
        // достаём из serivceCollection

        var start = (int)(object)Enum.GetValues<TEnum>().Min();
        var end = (int)(object)Enum.GetValues<TEnum>().Max();
        var builder = new BuilderContextFlow<TEnum>(serviceCollection, new RangeFlowComponents(start, end));

        builderFunc(builder);

        registryFlow.AddFlow<TEnum>(builder.Steps);
    }
}

[Obsolete] // как только
internal static class ConfigurationStateMachine
{
    public static StateMachine<TState, Trigger> BaseConfiguration<TState>(
        this StateMachine<TState, Trigger> stateMachine)
        where TState : struct, Enum
    {
        var enums = Enum.GetValues<TState>();

        for (var i = 1; i < enums.Length; i++)
        {
            var k = stateMachine.Configure(enums[i - 1]);
            k.Permit(Trigger.UserWantToContinue, enums[i]);
        }

        return stateMachine;
    }
}

public class RangeFlowComponents(int start, int end)
{
    private int _cur = start;
    private readonly int _start = start;

    public bool IsStart => _start == _cur;
    public int PrevHandler { get; set; } = start;

    public bool Empty => _cur > end;
    public int GetIdFreeComponent => _cur;

    public void Next() => _cur++;
}

internal record IHandlerInfo(IContextHandler ContextHandler, string number);