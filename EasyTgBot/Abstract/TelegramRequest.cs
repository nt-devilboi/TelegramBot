using Telegram.Bot.Types;

namespace EasyTgBot.Abstract;

public record TelegramRequest<TData>(TData Value, Update Update);