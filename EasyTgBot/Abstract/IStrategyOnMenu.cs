using System.Windows.Input;
using EasyTgBot.Entity;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace EasyTgBot.Abstract;

public interface IStrategyOnMenu
{
    public Task Handle(ChatContext context);
}

internal class MessageHandler : IContextHandler
{
    private readonly ITelegramBotClient _botClient;
    private readonly IContextRepository _contextRepository;
    private readonly Dictionary<string, Command> _commands;
    private readonly Dictionary<string, IContextHandler> _contexts;
    private readonly IStrategyOnMenu _strategyOnMenu;

    public MessageHandler(IEnumerable<Command> commands, IEnumerable<IHandlerInfo> handlerInfos,
        ITelegramBotClient botClient, IContextRepository contextRepository,
        IStrategyOnMenu strategyOnMenu)
    {
        _botClient = botClient;
        _contextRepository = contextRepository;
        _strategyOnMenu = strategyOnMenu;
        _commands = commands.ToDictionary(x => x.Trigger, x => x);
        _contexts = handlerInfos.ToDictionary(x => x.number, x => x.ContextHandler);
        _contexts.Add(BaseContextState.UserMenu.ToString(), this);
    }


    public async Task Handle(Update update, ChatContext context, IContextFactory contextFactory)
    {
        var text = update.Message.Text;
        if (!_commands.TryGetValue(text, out var command) &
            !_contexts.TryGetValue(context.State.ToString(), out var contextHandler))
            await _botClient.SendTextMessageAsync(update.Message.Chat.Id, "я не понял твоего сообщения");


        var oldState = context.State;
        if (command is { Priority: Priority.SystemCommand })
        {
            await command.Execute(update, context);
            await _contextRepository.Upsert(context);
        }

        else if (contextHandler != null && contextHandler != this)
        {
            await contextHandler.Handle(update, context, contextFactory);
            await _contextRepository.Upsert(context);
        }

        else if (command is { Priority: Priority.Command })
        {
            await command.Execute(update, context);
            await _contextRepository.Upsert(context);
        }
        else
        {
            await _botClient.SendTextMessageAsync(update.Message.Chat.Id, "Я ваще ничего не понял"); // сделать изменить
        }

        if (string.CompareOrdinal(context.State, oldState) != 0 &&
            _contexts.TryGetValue(context.State, out contextHandler))
        {
            await contextHandler.Enter(context, contextFactory);
        }
    }

    async Task IContextHandler.Enter(ChatContext context, IContextFactory _)
    {
        await _strategyOnMenu.Handle(context);
    }
}