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
    IServiceRegistry<ICommand> serviceServiceRegistry,
    IChatContextRepository chatContextRepository,
    IServiceRegistry<IContextHander> contextProcess)
{
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Update? update)
    {
        if (update?.Message == null) return new OkResult();


        var context = await chatContextRepository.Get(update.Message.Chat.Id) ?? NotAuthorized();

        var text = update.Message.Text;
        if (!serviceServiceRegistry.Contains(update.Message.Text) && contextProcess.Contains(context.State.ToString()))
        {
            await contextProcess.Get(context.State.ToString())
                .Handle(update, telegramBotClient, context);
        }

        else if (serviceServiceRegistry.Contains(update.Message.Text)) 
        {
            try
            {
                var command = serviceServiceRegistry.Get(text);
                await command.Execute(update, telegramBotClient, context);
            }
            catch (Exception e)
            {
                Console.WriteLine(e); //todo поставить логер.
                await telegramBotClient.SendTextMessageAsync(update.Message.Chat.Id, "Твоя команда меня сломала");
            }
        }
        else
        {
            await telegramBotClient.SendTextMessageAsync(update.Message.Chat.Id, "Я тебя не понимаю");
        }


        return new OkResult();
    }

    private ChatContext NotAuthorized()
    {
        return new ChatContext
        {
            State = (int)BaseContextState.Public,
            Id = Guid.NewGuid(),
            Payload = ""
        };
    }
}