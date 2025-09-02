using EasyTgBot.Abstract;
using EasyTgBot.Entity;
using EasyTgBot.Restored.Abstract;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace EasyTgBot.controller;

[ApiController]
[Route("/api/message/update")]
public class BotController(
    ITelegramBotClient telegramBotClient,
    ICommandCollection serviceCommandCollection,
    IChatRepository chatRepository,
    IContextProcess contextProcess)
{
    [HttpPost] //todo есть что рефакторить
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

        var context = await chatRepository.GetContext(request.Message.Chat.Id.ToString());

        if (!serviceCommandCollection.Contains(request.messageFromUser))
        {
            await contextProcess.Handle(request, telegramBotClient, context ?? NotAuthorized());
            return new OkResult();
        }

        var command = serviceCommandCollection.Get(request.messageFromUser);

        try
        {
            await command.Execute(request, telegramBotClient, context ?? NotAuthorized());
        }
        catch (Exception e)
        {
            Console.WriteLine(e); //todo поставить логер.
            await telegramBotClient.SendTextMessageAsync(request.Message.Chat.Id, "Твоя команда меня сломала");
        }

        return new OkResult();
    }

    private ChatContext NotAuthorized()
    {
        return new ChatContext
        {
            State = (int)ContextState.NotAuthenticated,
            Id = Guid.NewGuid(),
            Payload = ""
        };
    }
}