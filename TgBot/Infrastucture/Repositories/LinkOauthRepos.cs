using EasyOAuth;
using EasyOAuth.Abstraction;
using Microsoft.EntityFrameworkCore;
using TgBot.Domain.Entity;
using TgBot.Infrastucture.DataBase;

namespace TgBot.Infrastucture.Repositories;

public class LinkOauthRepos : TokenLinkRepositoryBase
{
    private readonly OAuthDb _oAuthDbContext;

    public LinkOauthRepos(OAuthDb oAuthDbContext)
    {
        _oAuthDbContext = oAuthDbContext;
    }


    public override async Task Add(string Oauth, string state, string id)
    {
        var linkOauth = new LinkOAuth()
        {
            chatId = id,
            State = state,
            OAuthName = Oauth
        };
        
        
        _oAuthDbContext.LinkOAuths.Add(linkOauth);
        await _oAuthDbContext.SaveChangesAsync();
    }

    public override Task<OAuthEntity> GetByExtraData(string extraData)
    {
        throw new NotImplementedException();
    }

    public override async Task<OAuthEntity> GetByState(string state)
    {
        return await _oAuthDbContext.LinkOAuths.FirstOrDefaultAsync(e => e.State == state);
    }
    
}