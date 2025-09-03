using EasyTgBot.Entity;

namespace EasyTgBot.Abstract;

public interface IChatRepository
{
    public Task Add(Chat chat);

    public Task<Chat> Get(long chatId);
}