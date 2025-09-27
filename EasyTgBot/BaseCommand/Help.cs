using EasyTgBot.Abstract;
using EasyTgBot.Entity;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace EasyTgBot.BaseCommand;

public class Help(List<InfoCommand> infoCommands, ITelegramBotClient botClient) : ICommand
{
    public string Trigger => "Что ты можешь";
    public string Desc { get; } = "Get All Commands";

    public Priority Priority { get; } = Priority.SystemCommand;

    public async Task Execute(Update update, ChatContext context)
    {
        await botClient.SendTextMessageAsync(update.Message.Chat.Id, string.Join("\n", infoCommands));
    }
}