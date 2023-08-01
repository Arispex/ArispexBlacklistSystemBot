using ArispexBlacklistSystemBot.Models;
using Microsoft.EntityFrameworkCore;

namespace ArispexBlacklistSystemBot.Data;

public class ApplicationDbContext: DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=app.db");
    }
    
    public DbSet<AuthorizedGroup> AuthorizedGroups { get; set; }
    
    public DbSet<AuthorizedUser> AuthorizedUsers { get; set; }
}