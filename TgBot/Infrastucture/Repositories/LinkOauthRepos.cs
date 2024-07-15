using EasyOAuth;
using EasyOAuth.Abstraction;
using Microsoft.EntityFrameworkCore;
using TgBot.Domain.Entity;
using TgBot.Infrastucture.DataBase;

namespace TgBot.Infrastucture.Repositories;

public class LinkOauthRepos : TokenLinkRepositoryBase
{
    private readonly DbOAuth DbContext;

    public LinkOauthRepos(DbOAuth dbContext)
    {
        DbContext = dbContext;
    }


    public override async Task Add(string Oauth, string state, string id)
    {
        var linkOauth = new LinkOAuth()
        {
            chatId = id,
            State = state,
            OAuthName = Oauth
        };
        
        
        DbContext.LinkOAuths.Add(linkOauth);
        await DbContext.SaveChangesAsync();
    }

    public override Task<OAuthEntity> GetByExtraData(string extraData)
    {
        throw new NotImplementedException();
    }

    public override async Task<OAuthEntity> GetByState(string state)
    {
        return await DbContext.LinkOAuths.FirstOrDefaultAsync(e => e.State == state);
    }
    
}