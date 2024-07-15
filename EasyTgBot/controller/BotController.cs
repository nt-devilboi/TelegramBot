using EasyTgBot.Abstract;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace EasyTgBot.controller;

[ApiController]
[Route("/api/message/update")]
public class BotController(ITelegramBotClient telegramBotClient, ICommandCollection serviceCommandCollection)
{
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Update? update)
    {
        if (update?.Message?.Text == null) return new OkResult();

        var result = update.Message.Parse();
        if (!result.Succeed())
        {
            await telegramBotClient.SendTextMessageAsync(update.Message.Chat.Id, result.Error);
            return new OkResult();
        }

        var request = result.Value; 
        if (!serviceCommandCollection.Contains(request.CommandName))
        {
            await telegramBotClient.SendTextMessageAsync(request.Message.Chat.Id, "I don't known this command");;
            return new OkResult();
        }

        var command = serviceCommandCollection.Get(request.CommandName);
        await command.Execute(request, telegramBotClient);

        return new OkResult();
    }
}