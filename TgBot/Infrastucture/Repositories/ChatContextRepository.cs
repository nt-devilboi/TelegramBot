using EasyTgBot.Entity;
using EasyTgBot.Restored.Abstract;
using TgBot.Domain.Entity;
using TgBot.Infrastucture.DataBase;

namespace TgBot.Infrastucture.Repositories;

public class ChatContextRepository(ChatDb dbContext) : IChatContextRepository
{
    public async Task Upsert(ChatContext chatContext)
    {
        var context = await dbContext.ChatContexts.FindAsync(chatContext.Id);

        if (context == null) dbContext.ChatContexts.Add(chatContext);
        else dbContext.Entry(context).CurrentValues.SetValues(chatContext);

        await dbContext.SaveChangesAsync();
    }
}