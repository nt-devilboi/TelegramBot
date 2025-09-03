using EasyTgBot.Abstract;
using EasyTgBot.Entity;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace EasyTgBot.BaseCommand;

public class Help(List<InfoCommand> infoCommands) : ICommand
{
    public string Name => "Что ты можешь";
    public string Desc { get; } = "Get All Commands";

    public async Task Execute(Update update, ITelegramBotClient bot, ChatContext context)
    {
        await bot.SendTextMessageAsync(update.Message.Chat.Id, string.Join("\n", infoCommands));
    }
}