using EasyTgBot.Entity;

namespace EasyTgBot;

public static class ContextExtension
{
    public static bool InUserAccount(this ChatContext context)
    {
        return context.State == (int)ContextState.UserAccount;
    }

    public static bool InPublic(this ChatContext context)
    {
        return context.State == (int)ContextState.Public;
    }


    public static bool InFlow(this ChatContext context)
    {
        return !context.InUserAccount() && !context.InPublic();
    }
}