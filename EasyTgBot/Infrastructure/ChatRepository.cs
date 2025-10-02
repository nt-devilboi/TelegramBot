using EasyTgBot.Abstract;
using EasyTgBot.Entity;

namespace EasyTgBot.Infrastructure;

public class ChatRepository(ChatDb telegramDb) : IChatRepository
{
    public async Task Add(Chat chat)
    {
        await telegramDb.Chat.AddAsync(chat);
        await telegramDb.SaveChangesAsync();
    }

    public async Task<Chat?> Get(long chatId)
    {
        var chatLinkToken = await telegramDb.Chat.FindAsync(chatId);
        return chatLinkToken;
    }
}