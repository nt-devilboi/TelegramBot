using System.Net;
using System.Net.Mail;
using EasyTgBot.Abstract;
using MediatR;
using MyBotTg.Bot;
using Telegram.Bot;
using TgBot.Application;
using TgBot.Repositories;
using Vostok.Logging.Abstractions;

namespace TgBot.Commands;

public class SyncMails(IMailRepository mails, ILog log, IMediator mediator, IApartmentRepo aparts)
    : CommandTgBase
{
    public string Name { get; } = "/syncMails";
    public override string Desc { get; } = "if you want to get info about apart to email use this command";

    public override async Task Execute(ITgRequest? request, ITelegramBotClient bot)
    {
        var chatId = request.Message.Chat.Id;
        var mailsByChatId = await mails.GetAllMailByChatId(chatId);

        var from = new MailAddress("clashofnaks@gmail.com", "nikita");
        var smtp = new SmtpClient("smtp.gmail.com", 587)
        {
            Credentials = new NetworkCredential("clashofnaks@gmail.com", "chsk rupp emre epcn"),
            EnableSsl = true
        };
        
        var apart = await aparts.GetAll(chatId);
        var result= await mediator.Send(new GetInfoApart(apart));
        if (result.IsEmpty)
        {
            await bot.SendTextMessageAsync(chatId, "you didn't add apart");
            return;
        }

        var infoAparts = result.ResponseDataAparts;
        foreach (var mail in mailsByChatId)
        {
            log.Info($"send to {mail.Address}");
            var to = new MailAddress(mail.Address);
            var m = new MailMessage(from, to);
         
            m.Subject = "Актуальные цены на квартиры";
            m.Body = string.Join(Environment.NewLine, infoAparts.Select(x => x.ToString()));


            smtp.Send(m);
            log.Info($"sent to {mail.Address}");
        }

        await bot.SendTextMessageAsync(chatId, "message send to all Email");
        log.Info($"sent to all mails");
    }
}