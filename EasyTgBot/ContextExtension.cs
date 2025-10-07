using EasyTgBot.Entity;

namespace EasyTgBot;

public static class ContextExtension
{
    public static bool InUserAccount(this ChatContext context)
    {
        return context.State == (int)BaseContextState.UserMenu;
    }

    public static bool InPublic(this ChatContext context)
    {
        return context.State == (int)BaseContextState.Public;
    }


    public static bool InFlow(this ChatContext context)
    {
        return !context.InUserAccount() && !context.InPublic();
    }

    public static void ToUserAccount(this ChatContext context)
    {
        context.State = (int)BaseContextState.UserMenu;
    }
}