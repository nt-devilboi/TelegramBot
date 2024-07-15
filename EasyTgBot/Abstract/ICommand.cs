using Telegram.Bot;

namespace EasyTgBot.Abstract;

public interface
    ICommandTg //todo: прописать абстарктынй класс, который будет по названию класса менять свойства "Name" 
{
    public string Name { get; }

    public string Desc { get; }
    public Task Execute(ITgRequest? request, ITelegramBotClient bot);
}