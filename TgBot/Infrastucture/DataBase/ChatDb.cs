using EasyTgBot.Entity;
using Microsoft.EntityFrameworkCore;
using TgBot.Domain.Entity;

namespace TgBot.Infrastucture.DataBase;

public class ChatDb : DbContext
{
    public DbSet<TelegramOAuth> LinkOAuths { get; set; }
    public DbSet<Chat> Chat { get; set; }
    public DbSet<ChatContext> ChatContexts { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseInMemoryDatabase("FakeDbContext");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Chat>(x =>
        {
            x.HasKey(e => e.Id);

            x.HasOne<ChatContext>()
                .WithOne()
                .HasForeignKey<ChatContext>(x => x.Id)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ChatContext>(x => x.HasKey(x => x.Id));

        base.OnModelCreating(modelBuilder);
    }
}