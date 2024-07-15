using EasyTgBot.Abstract;
using MyBotTg.Bot;
using Telegram.Bot;
using TgBot.Domain.Entity;
using Vostok.Logging.Abstractions;

namespace TgBot.Commands;

public class AddEmail(ILog log, IMailRepository mailRepository) : CommandTgBase
{
    public string Name { get; } = "/addMail";
    public override string  Desc { get; } = "addMail {mail}";
    
    public override async Task Execute(ITgRequest? request, ITelegramBotClient bot) //addEmail and AddApart are obviously the same
    {
        var chatId = request.Message.Chat.Id;
        var mailAddress = request.ExtraData;
        var user = Mail.From(mailAddress, chatId);
        var mails = await mailRepository.GetAllMailByChatId(chatId);
        if (mails.Any(mail => mail.Address == mailAddress))
        {
            await bot.SendTextMessageAsync(chatId, "this email added yet");
            return;
        }

        throw new AggregateException();
        await mailRepository.Add(user);
        await bot.SendTextMessageAsync(chatId, "added email {");
        log.Info($"addEmail: {mailAddress} in chatId: {chatId}");
    }
}