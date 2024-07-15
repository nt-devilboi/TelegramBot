using EasyTgBot.Abstract;
using MediatR;
using MyBotTg.Bot;
using Telegram.Bot;
using TgBot.Application;
using TgBot.Repositories;
using Vostok.Logging.Abstractions;

namespace TgBot.Commands;

public class GetApart(IApartmentRepo apartments, ISender mediator) : CommandTgBase
{

    public string Name { get; } = "/getApart";
    public override string Desc { get; } = "Get all Subscribed apartment";
    
    public override async Task Execute(ITgRequest? request, ITelegramBotClient bot)
    {
        var aparts = await apartments.GetAll(request.Message.Chat.Id);

        var result = await mediator.Send(new GetInfoApart(aparts));
        if (result.IsEmpty)
        {
            await bot.SendTextMessageAsync(request.Message.Chat.Id, "you must add apart before use this command");
            return;
        }

        var infoPriceApart = result.ResponseDataAparts; 
        await bot.SendTextMessageAsync(request.Message.Chat.Id, string.Join(Environment.NewLine,infoPriceApart.Select(x => x.ToString())));
    }
}