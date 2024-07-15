using EasyTgBot.Abstract;
using Telegram.Bot;
using TgBot.Domain.Entity;
using TgBot.Repositories;
using Vostok.Logging.Abstractions;

namespace TgBot.Commands;

public class AddApartment(IApartmentRepo _apartments, ILog log) : CommandTgBase
{
    public string Name { get; } = "/addApart";
    public override string Desc { get; } = "addApart {UrlApart} add Apart to Receiving info about one";

    public override async Task Execute(ITgRequest? request, ITelegramBotClient bot)
    {
        var chatId = request.Message.Chat.Id;
        var url = request.ExtraData;
        var apartment = Apartment.From(url, chatId);

        var apartments = await _apartments.GetAll(chatId);
        if (apartments.Any(x => x.UrlApart == apartment.UrlApart))
        {
            await bot.SendTextMessageAsync(chatId, "this apartment added yet");
            return;
        }
        
        await _apartments.Add(apartment);
        await bot.SendTextMessageAsync(chatId, "Apart Added");
        log.Info($"added Apart with url: {url}");
        
    }
}