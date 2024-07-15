namespace EasyTgBot.Result;

public class Result<T>
{
    private bool _successed;
    public Result(T value)
    {
        Value = value;
        _successed = true;
    }

    public Result(string error)
    {
        Error = error;
    }
    
    public bool Succeed() => _successed;
    public string Error { get; }
    public T Value { get; }
    public static implicit operator Result<T>(T value) => new(value);
}