using cloud.Model;
using Microsoft.EntityFrameworkCore;

namespace cloud.Database;

public class AppDbContext: DbContext {
    public DbSet<User> Users { get; set; } // Example DbSet for a 'User' entity
    public DbSet<Token> Tokens { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
    //     optionsBuilder.UseNpgsql( "Host=localhost;Username=postgres;Password=root;Database=dentiste" );
    // }
    
}