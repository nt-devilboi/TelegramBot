namespace EasyTgBot.Abstract;

public interface IDetailContext<TPayload, out TState> where TState : Enum
{
    public TPayload Payload { get; }
    public IDetailContext<TPayload, TState> PostPayload(TPayload payload);
    public IDetailContext<TPayload, TState> SaveChanges();
    public IDetailContext<TPayload, TState> NextState();

    public bool WasChanged { get; }
    public TState State { get; }
}