namespace EasyTgBot.Entity;

public class ChatContext
{
    public Guid Id { get; set; }
    public string State { get; set; }
    public string? Payload { get; set; }
    public long ChatId { get; set; }


    public static ChatContext CreateInAccountContext(long chatId)
    {
        return new ChatContext
        {
            State = BaseContextState.UserMenu.ToString(),
            Id = Guid.NewGuid(),
            ChatId = chatId
        };
    }
}

internal enum BaseContextState
{
    Public,
    UserMenu
}