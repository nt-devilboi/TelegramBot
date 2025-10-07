using EasyTgBot.Entity;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace EasyTgBot.Abstract;

public interface IUpdateProcess
{
    public Task Update(Update update);
}

internal class UpdateProcess(
    ITelegramBotClient telegramBotClient,
    MessageHandler messageHandler,
    IContextRepository contextRepository,
    IContextFactory contextFactory)
    : IUpdateProcess
{
    public async Task Update(Update update)
    {
        if (update.Message?.Text != null)
        {
            var context = await contextRepository.Get(update.Message.Chat.Id) ?? NotAuthorized();
            await messageHandler.Handle(update, context, contextFactory);
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