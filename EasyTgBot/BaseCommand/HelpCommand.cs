using EasyTgBot.Abstract;
using Telegram.Bot;

namespace EasyTgBot.BaseCommand;

public class HelpCommand : CommandTgBase
{
    public override string  Name { get; } = "/help";
    public override string Desc { get; } = "Get All Commands";
    private readonly List<InfoCommand> _infoCommands;

    public HelpCommand(List<InfoCommand> infoCommands)
    {
        _infoCommands = infoCommands;
    }

    public override async Task  Execute(ITgRequest? request, ITelegramBotClient bot)
    {
        await bot.SendTextMessageAsync(request.Message.Chat.Id,string.Join("\n",_infoCommands));
    }
}