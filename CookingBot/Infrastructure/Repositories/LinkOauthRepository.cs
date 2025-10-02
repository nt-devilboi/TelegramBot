using CookingBot.Domain.Entity;
using CookingBot.Infrastructure.DataBase;
using EasyOAuth.Abstraction;
using Microsoft.EntityFrameworkCore;

namespace CookingBot.Infrastructure.Repositories;

public class LinkOauthRepository(ChatTelegramDb chatTelegramDbContext) : ITokenLinkRepository
{
    public async Task Add(string Oauth, string state, string id)
    {
        var linkOauth = new TelegramOAuth
        {
            chatId = long.Parse(id),
            State = state,
            OAuthName = Oauth
        };


        chatTelegramDbContext.LinkOAuths.Add(linkOauth);
        await chatTelegramDbContext.SaveChangesAsync();
    }

    public Task<OAuthEntity> GetByExtraData(string extraData)
    {
        throw new NotImplementedException();
    }

    public async Task<OAuthEntity> GetByState(string state)
    {
        return await chatTelegramDbContext.LinkOAuths.FirstOrDefaultAsync(e => e.State == state);
    }

    public async Task Remove(OAuthEntity oAuthEntity)
    {
        chatTelegramDbContext.LinkOAuths.Remove(oAuthEntity as TelegramOAuth);
        await chatTelegramDbContext.SaveChangesAsync();
    }
}