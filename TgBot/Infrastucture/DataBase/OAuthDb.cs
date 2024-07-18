using Microsoft.EntityFrameworkCore;
using TgBot.Domain.Entity;

namespace TgBot.Infrastucture.DataBase;

public class OAuthDb : DbContext
{
    public DbSet<LinkOAuth> LinkOAuths { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseInMemoryDatabase("FakeDbContext");
    }
}