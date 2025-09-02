namespace EasyTgBot.Result;

public class Result<T>
{
    private readonly bool _successed;

    public Result(T value)
    {
        Value = value;
        _successed = true;
    }

    public Result(string error)
    {
        Error = error;
    }

    public string Error { get; }
    public T Value { get; }

    public bool Succeed()
    {
        return _successed;
    }

    public static implicit operator Result<T>(T value)
    {
        return new Result<T>(value);
    }
}