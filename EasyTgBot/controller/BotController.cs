using EasyTgBot.Abstract;
using EasyTgBot.Entity;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace EasyTgBot.controller;

[ApiController]
[Route("/api/message/update")]
public class BotController(
    ITelegramBotClient telegramBotClient,
    IUpdateProcess updateProcess)
{
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Update? update)
    {
        if (update?.Message == null) return new OkResult();

        try
        {
            await updateProcess.Update(update);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            await telegramBotClient.SendTextMessageAsync(update.Message.Chat.Id, "Я сломался из за команды");
        }

        return new OkResult();
    }
}