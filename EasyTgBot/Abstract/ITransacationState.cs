using EasyTgBot.Entity;

namespace EasyTgBot.Abstract;

public interface ITransactionService
{
    public void NextState(ChatContext context);
}