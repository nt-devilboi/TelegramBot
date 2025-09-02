using EasyTgBot.Entity;

namespace EasyTgBot.Restored.Abstract;

public interface IChatRepository
{
    public Task Add(Chat chat);

    public Task<Chat> Get(string chatId);

    public Task<ChatContext?> GetContext(string chatId);
}