using System.Net;
using System.Net.Mail;
using MyBotTg.Bot;
using Telegram.Bot;
using TgBot.controller.BotController.Services;
using TgBot.Domain.Entity;
using TgBot.Repositories;
using Vostok.Logging.Abstractions;

namespace TgBot.Commands;

public class AddApartment : ICommand
{
    private readonly IMailRepository _mails;
    private readonly IApartmentRepo Apartments;
    private ILog _log;
    public AddApartment(IMailRepository mails, IApartmentRepo apartments, ILog log)
    {
        _mails = mails;
        Apartments = apartments;
        _log = log;
    }
    
    public string Name { get; } = "/addApart";
    public string desc { get; } = "addApart {UrlApart} add Apart to Receiving info about one";

    public async Task Execute(IRequest? request, ITelegramBotClient bot)
    {
        var chatId = request.Message.Chat.Id;
        var url = request.ExtraData;
        var apartment = Apartment.From(url, chatId);

        var apartments = await Apartments.GetAll(chatId);
        if (apartments.Any(x => x.UrlApart == apartment.UrlApart))
        {
            await bot.SendTextMessageAsync(chatId, "this apartment added yet");
            return;
        }
        
        await Apartments.Add(apartment);
        await bot.SendTextMessageAsync(chatId, "Apart Added");
        _log.Info($"added Apart with url: {url}");
        
    }
}