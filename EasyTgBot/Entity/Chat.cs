namespace EasyTgBot.Entity;

public class Chat
{
    public Guid Id { get; set; }
    public string token { get; set; }
    public string ChatId { get; set; }

    public ChatContext ChatContext { get; set; }
}