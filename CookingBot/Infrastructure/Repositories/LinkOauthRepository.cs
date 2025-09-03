using CookingBot.Domain.Entity;
using CookingBot.Infrastucture.DataBase;
using EasyOAuth.Abstraction;
using Microsoft.EntityFrameworkCore;

namespace CookingBot.Infrastucture.Repositories;

public class LinkOauthRepository(ChatDb chatDbContext) : TokenLinkRepositoryBase
{
    public override async Task Add(string Oauth, string state, string id)
    {
        var linkOauth = new TelegramOAuth
        {
            chatId = long.Parse(id),
            State = state,
            OAuthName = Oauth
        };


        chatDbContext.LinkOAuths.Add(linkOauth);
        await chatDbContext.SaveChangesAsync();
    }

    public override Task<OAuthEntity> GetByExtraData(string extraData)
    {
        throw new NotImplementedException();
    }

    public override async Task<OAuthEntity> GetByState(string state)
    {
        return await chatDbContext.LinkOAuths.FirstOrDefaultAsync(e => e.State == state);
    }

    public override async Task Remove(OAuthEntity oAuthEntity)
    {
        chatDbContext.LinkOAuths.Remove(oAuthEntity as TelegramOAuth);
        await chatDbContext.SaveChangesAsync();
    }
}