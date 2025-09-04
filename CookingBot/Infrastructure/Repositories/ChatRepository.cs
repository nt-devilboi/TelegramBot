using CookingBot.Infrastructure.DataBase;
using EasyTgBot.Abstract;
using EasyTgBot.Entity;
using Vostok.Logging.Abstractions;

namespace CookingBot.Infrastucture.Repositories;

public class ChatRepository(ChatDb db, ILog log = null) : IChatRepository
{
    public async Task Add(Chat chat)
    {
        await db.Chat.AddAsync(chat);
        log.Info($"added {chat}");
        await db.SaveChangesAsync();
    }

    public async Task<Chat?> Get(long chatId)
    {
        var chatLinkToken = await db.Chat.FindAsync(chatId);
        return chatLinkToken;
    }
}