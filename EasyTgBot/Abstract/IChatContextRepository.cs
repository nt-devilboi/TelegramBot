using EasyTgBot.Entity;

namespace EasyTgBot.Abstract;

public interface IChatContextRepository
{
    Task Upsert(ChatContext chatContext);

    Task<ChatContext?> Get(long chatId);
}