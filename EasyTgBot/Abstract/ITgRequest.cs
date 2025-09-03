using Telegram.Bot.Types;

namespace EasyTgBot.Abstract;

public interface ITgRequest<TData>
{
    TData Value { get; }
    Update Update { get; }
}

public class TextRequest : ITgRequest<string>
{
    public string Value { get; init; }
    public Update Update { get; init; }
}