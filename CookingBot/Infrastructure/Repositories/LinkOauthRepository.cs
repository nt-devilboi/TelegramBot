using CookingBot.Domain.Entity;
using CookingBot.Infrastructure.DataBase;
using EasyOAuth.Abstraction;
using Microsoft.EntityFrameworkCore;

namespace CookingBot.Infrastructure.Repositories;

public class LinkOauthRepository(ChatDb chatDbContext) : ITokenLinkRepository
{
    public async Task Add(string Oauth, string state, string id)
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

    public Task<OAuthEntity> GetByExtraData(string extraData)
    {
        throw new NotImplementedException();
    }

    public async Task<OAuthEntity> GetByState(string state)
    {
        return await chatDbContext.LinkOAuths.FirstOrDefaultAsync(e => e.State == state);
    }

    public async Task Remove(OAuthEntity oAuthEntity)
    {
        chatDbContext.LinkOAuths.Remove(oAuthEntity as TelegramOAuth);
        await chatDbContext.SaveChangesAsync();
    }
}