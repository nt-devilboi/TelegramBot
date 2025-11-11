using BotOrchestriX.Infrastructure;
using CookingBot.Application.Flows.Friends.InContext;
using CookingBot.Domain.Entity;
using CookingBot.Domain.Payloads;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace CookingBot.Infrastructure.DataBase;

public class ChatTelegramDb(IOptions<PostgresEntryPointOptions> options) : ChatDb
{
    private readonly string ConnectionString = options.Value.ConnString;
    public DbSet<TelegramOAuth> LinkOAuths { get; set; }
    public DbSet<Recipe> Recipes { get; set; }
    // public DbSet<Friend> Friends { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    { 
        optionsBuilder.UseNpgsql(ConnectionString);
        // optionsBuilder.UseInMemoryDatabase("FakeDbContext");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        /*modelBuilder.Entity<Recipe>()
            .Property(x => x.Ingredients)
            .HasColumnType("jsonb")
            .HasConversion<string>();*/

        modelBuilder.Entity<Recipe>()
            .Property(x => x.Ingredients)
            .HasColumnType("jsonb")
            .HasConversion<string>(x => JsonConvert.SerializeObject(x),
                x => JsonConvert.DeserializeObject<Dictionary<string, IngredientDetail>>(x));
        modelBuilder.Ignore<IngredientDetail>();
        base.OnModelCreating(modelBuilder);
    }
}