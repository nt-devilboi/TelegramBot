using EasyTgBot.Abstract;
using Telegram.Bot;

namespace TgBot.Commands;

public class StartHH : ICommandTg
{
    public string Name { get; } = "/startHH";
    public string Desc { get; } = "Start Telegram bot";

    public async Task Execute(ITgRequest? request, ITelegramBotClient bot)
    {
        var chatId = request.Message.Chat.Id;
        await bot.SendTextMessageAsync(chatId, "bla bla bla my command");
    }
}