using EasyTgBot.Entity;
using Microsoft.EntityFrameworkCore;

namespace EasyTgBot.Infrastructure;

public abstract class ChatDb : DbContext
{
    public DbSet<Chat> Chat { get; set; }
    public DbSet<ChatContext> ChatContexts { get; set; }
}