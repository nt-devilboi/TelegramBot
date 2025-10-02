using CookingBot.Domain.Entity;
using CookingBot.Domain.Payloads;
using EasyTgBot.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace CookingBot.Infrastructure.DataBase;

public class ChatTelegramDb(IOptions<PostgresEntryPointOptions> options) : ChatDb
{
    public DbSet<TelegramOAuth> LinkOAuths { get; set; }
    public DbSet<Recipe> Recipes { get; set; }
    private readonly string ConnectionString = options.Value.ConnString;


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