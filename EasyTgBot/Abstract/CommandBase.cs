using Telegram.Bot;

namespace EasyTgBot.Abstract;

public abstract class CommandTgBase : ICommandTg
{
    private string _name;

    public virtual string Name
    {
        get
        {
            if (_name == null) _name = '/' + GetNameClass(); //todo: сделать более красиво чем есть сейчас 
            
            return _name;
        }
    }

    public abstract string Desc { get; }
    public abstract Task Execute(ITgRequest? request, ITelegramBotClient bot);

    private string GetNameClass()
    {
        return GetType().Name.Replace("Command", "");
    }
}