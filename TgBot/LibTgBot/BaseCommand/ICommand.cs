using Telegram.Bot;

namespace TgBot.controller.BotController.Services;

public interface ICommand //todo: прописать абстарктынй класс, который будет по названию класса менять свойства "Name" 
{
    public string Name { get; }
    
    public string desc { get; }
    public Task Execute(IRequest? request, ITelegramBotClient bot);
}