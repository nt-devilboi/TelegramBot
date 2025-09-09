using EasyTgBot.Abstract;
using EasyTgBot.ServiceCommand;
using Microsoft.Extensions.DependencyInjection;

namespace EasyTgBot;

public static class ContextAddExtension
{
    public static void AddContext<TEnum>(this IServiceCollection serviceCollection,
        Action<BuilderRegistryContext> builderFunc) where TEnum : struct, Enum
    {
        // достаём из serivceCollection

        var start = (int)(object)Enum.GetValues<TEnum>().Min();
        var end = (int)(object)Enum.GetValues<TEnum>().Max();
        var builder = new BuilderRegistryContext(serviceCollection, start, end);

        builderFunc(builder);
    }
}

public class BuilderRegistryContext
{
    private readonly IServiceCollection _collection;
    private int _cur;
    private readonly int _end;

    internal BuilderRegistryContext(IServiceCollection collection,
        int cur, int end)
    {
        _collection = collection;
        _cur = cur;
        _end = end;
    }


    public BuilderRegistryContext AddHandler<TContextHandler>() where TContextHandler : class
    {
        if (_cur > _end)
        {
            throw new ArgumentException("capacity for handler is exhausted");
        }

        _collection.AddScoped<TContextHandler>(x =>
            (TContextHandler)ActivatorUtilities.CreateInstance(x, typeof(TContextHandler)));

        AddHandlerInfo<TContextHandler>(_cur.ToString());
        _cur++;

        return this;
    }

    private void AddHandlerInfo<TContextHandler>(string cur) where TContextHandler : class
    {
        _collection.AddScoped<HandlerInfo>(x =>
            new HandlerInfo((IContextHander)x.GetService(typeof(TContextHandler)), cur));
    }
}

internal record HandlerInfo(IContextHander ContextHander, string number);