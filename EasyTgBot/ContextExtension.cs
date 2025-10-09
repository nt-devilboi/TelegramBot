using EasyTgBot.Entity;

namespace EasyTgBot;

public static class ContextExtension
{
    public static bool InUserAccount(this ChatContext context)
    {
        return context.State == BaseContextState.UserMenu.ToString();
    }

    public static bool InPublic(this ChatContext context)
    {
        return context.State == BaseContextState.Public.ToString();
    }


    public static bool InFlow(this ChatContext context)
    {
        return !context.InUserAccount() && !context.InPublic();
    }

    public static void ToUserAccount(this ChatContext context)
    {
        context.State = BaseContextState.UserMenu.ToString();
    }
}