using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Options;

namespace CookingBot.Infrastructure.DataBase;

public class ChatDbfactory : IDesignTimeDbContextFactory<ChatTelegramDb>
{
    public ChatTelegramDb CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("CONN_STRING");
        var db = new PostgresEntryPointOptions()
        {
            ConnString = connectionString
        };
        
        return new ChatTelegramDb(Options.Create(db));
    }
}