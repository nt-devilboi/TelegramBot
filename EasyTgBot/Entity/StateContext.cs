namespace EasyTgBot.Entity;

public class ChatContext
{
    public Guid Id { get; set; }
    public int State { get; set; }
    public string? Payload { get; set; }
    public long ChatId { get; set; }


    public static ChatContext CreateInAccountContext(long chatId)
    {
        return new ChatContext
        {
            State = (int)ContextState.UserAccount,
            Id = Guid.NewGuid(),
            ChatId = chatId
        };
    }
}

internal enum ContextState
{
    Public,
    UserAccount
}