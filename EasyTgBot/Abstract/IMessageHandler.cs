using EasyTgBot.Entity;
using Telegram.Bot;

namespace EasyTgBot.Abstract;

public interface IMessageHandler
{
    public Task Handle(TelegramRequest<string> update, ChatContext context);
}

internal class MessageHandler : IMessageHandler
{
    private readonly ITelegramBotClient _botClient;
    private readonly IContextFactory _contextFactory;
    private readonly IContextRepository _contextRepository;
    private readonly Dictionary<string, ICommand> _commands;
    private readonly Dictionary<string, IContextHandler> _contexts;

    public MessageHandler(IEnumerable<ICommand> commands, IEnumerable<IHandlerInfo> handlerInfos,
        ITelegramBotClient botClient, IContextFactory contextFactory, IContextRepository contextRepository)
    {
        _botClient = botClient;
        _contextFactory = contextFactory;
        _contextRepository = contextRepository;
        _commands = commands.ToDictionary(x => x.Trigger, x => x);
        _contexts = handlerInfos.ToDictionary(x => x.number, x => x.ContextHandler);
    }

    // в принципе у обоих сущностей один интерфейс.
    public async Task Handle(TelegramRequest<string> update, ChatContext context)
    {
        if (!_commands.TryGetValue(update.Value, out var command) &
             !_contexts.TryGetValue(context.State.ToString(), out var contextHandler))
            await _botClient.SendTextMessageAsync(update.GetChatId(), "я не понял твоего сообщения");


        var oldState = context.State;
        if (command is { Priority: Priority.SystemCommand })
        {
            await command.Execute(update.Update, context);
            await _contextRepository.Upsert(context);
            return;
        }

        if (contextHandler != null)
        {
            await contextHandler.Handle(update.Update, context, _contextFactory);
            await _contextRepository.Upsert(context);
        }

        if (command is { Priority: Priority.Command })
        {
            await command.Execute(update.Update, context);
            await _contextRepository.Upsert(context);
        }

        if (context.State != oldState && _contexts.TryGetValue(context.State.ToString(), out contextHandler))
        {
            await contextHandler.Enter(context, _contextFactory);
        }
    }
}