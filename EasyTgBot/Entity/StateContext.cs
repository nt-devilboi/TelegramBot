namespace EasyTgBot.Entity;

public class ChatContext
{
    public Guid Id { get; set; }
    public int State { get; set; }
    public string? Payload { get; set; }

    public long ChatId { get; set; }
}

public enum ContextState
{
    NotAuthenticated,
    Menu
}