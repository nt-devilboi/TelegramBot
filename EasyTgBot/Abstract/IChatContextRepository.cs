using EasyTgBot.Entity;

namespace EasyTgBot.Restored.Abstract;

public interface IChatContextRepository
{
    Task Upsert(ChatContext chatContext);
}