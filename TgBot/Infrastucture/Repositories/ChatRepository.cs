using EasyTgBot.Entity;
using EasyTgBot.Restored.Abstract;
using Microsoft.EntityFrameworkCore;
using TgBot.Domain.Entity;
using TgBot.Infrastucture.DataBase;
using Vostok.Logging.Abstractions;

namespace TgBot.Infrastucture.Repositories;

public class ChatRepository(ChatDb db, ILog log = null) : IChatRepository
{
    public async Task Add(Chat chat)
    {
        await db.Chat.AddAsync(chat);
        log.Info($"added {chat}");
        await db.SaveChangesAsync();
    }

    public async Task<Chat?> Get(string chatId)
    {
        var chatLinkToken = await db.Chat.FirstOrDefaultAsync(x => x.ChatId == chatId);
        return chatLinkToken;
    }

    public async Task<ChatContext?> GetContext(string chatId)
    {
        var chat = await db.Chat.Include(chat => chat.ChatContext)
            .FirstOrDefaultAsync(x => x.ChatId == chatId);


        return chat?.ChatContext;
    }
}