using Telegram.Bot.Types;
using TgBot.Domain.Entity;

namespace MyBotTg.Bot;

public interface IMailRepository
{
    public Task Add(Mail mail);
    public Task<Mail[]> GetAllMailByChatId(long chatId);
    
    
    public Task Remove(Mail mail);
}