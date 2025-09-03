using CookingBot.Infrastucture.DataBase;
using EasyTgBot.Abstract;
using EasyTgBot.Entity;
using Microsoft.EntityFrameworkCore;

namespace CookingBot.Infrastructure.Repositories;

public class ChatContextRepository(ChatDb dbContext) : IChatContextRepository
{
    public async Task Upsert(ChatContext chatContext)
    {
        var context = await dbContext.ChatContexts.FindAsync(chatContext.Id);

        if (context == null) dbContext.ChatContexts.Add(chatContext);
        else dbContext.Entry(context).CurrentValues.SetValues(chatContext);

        await dbContext.SaveChangesAsync();
    }

    public async Task<ChatContext?> Get(long chatId)
    {
        var chatContext = await dbContext.ChatContexts.FirstOrDefaultAsync(x => x.ChatId == chatId);

        return chatContext;
    }
}