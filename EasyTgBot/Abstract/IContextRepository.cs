using EasyTgBot.Entity;

namespace EasyTgBot.Abstract;

public interface IContextRepository
{
    Task Upsert(ChatContext chatContext);

    Task<ChatContext?> Get(long chatId);
}