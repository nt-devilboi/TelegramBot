using MyBotTg.Bot;
using Telegram.Bot;
using TgBot.controller.BotController.Services;
using TgBot.Domain.Entity;
using Vostok.Logging.Abstractions;

namespace TgBot.Commands;

public class AddEmail : ICommand
{
    private readonly IMailRepository _mailRepository;
    private readonly ILog _log;

    public AddEmail(ILog log, IMailRepository mailRepository)
    {
        _log = log;
        _mailRepository = mailRepository;
    }

    
    public string Name { get; } = "/addMail";
    public string desc { get; } = "addMail {mail}";
    
    
    public async Task Execute(IRequest? request, ITelegramBotClient bot) //addEmail and AddApart are obviously the same
    {
        var chatId = request.Message.Chat.Id;
        var mailAddress = request.ExtraData;
        var user = Mail.From(mailAddress, chatId);

        var mails = await _mailRepository.GetAllMailByChatId(chatId);
        if (mails.Any(mail => mail.Address == mailAddress))
        {
            await bot.SendTextMessageAsync(chatId, "this email added yet");
            return;
        }
        
        await _mailRepository.Add(user);
        await bot.SendTextMessageAsync(chatId, "added email {");
        _log.Info($"addEmail: {mailAddress} in chatId: {chatId}");
    }
}