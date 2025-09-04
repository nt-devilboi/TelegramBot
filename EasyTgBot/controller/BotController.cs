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
    ICommandCollection serviceCommandCollection,
    IChatContextRepository chatContextRepository,
    IContextProcess contextProcess)
{
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Update? update)
    {
        if (update?.Message == null) return new OkResult();


        var context = await chatContextRepository.Get(update.Message.Chat.Id);

        var text = update.Message.Text;
        if (text == null || !serviceCommandCollection.Contains(update.Message.Text))
        {
            await contextProcess.Handle(update, telegramBotClient, context ?? NotAuthorized());
            return new OkResult();
        }

        var command = serviceCommandCollection.Get(text);

        try
        {
            await command.Execute(update, telegramBotClient, context ?? NotAuthorized());
        }
        catch (Exception e)
        {
            Console.WriteLine(e); //todo поставить логер.
            await telegramBotClient.SendTextMessageAsync(update.Message.Chat.Id, "Твоя команда меня сломала");
        }

        return new OkResult();
    }

    private ChatContext NotAuthorized()
    {
        return new ChatContext
        {
            State = (int)ContextState.Public,
            Id = Guid.NewGuid(),
            Payload = ""
        };
    }
}