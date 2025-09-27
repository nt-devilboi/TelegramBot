using EasyTgBot.Entity;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace EasyTgBot.Abstract;

public interface IUpdateProcess
{
    public Task Update(Update update);
}

public class UpdateProcess(
    ITelegramBotClient telegramBotClient,
    IMessageHandler messageHandler,
    IContextRepository contextRepository)
    : IUpdateProcess
{
    public async Task Update(Update update)
    {
        if (update.Message?.Text != null)
        {
            var context = await contextRepository.Get(update.Message.Chat.Id) ?? NotAuthorized();
            await messageHandler.Handle(update.AsRequestWithText(), context);
        }
        else
        {
            await telegramBotClient.SendTextMessageAsync(update.Message.Chat.Id, "Я умею понимать только сообщения");
        }
    }


    private ChatContext NotAuthorized()
    {
        return new ChatContext
        {
            State = (int)BaseContextState.Public,
            Id = Guid.NewGuid(),
            Payload = ""
        };
    }
}