using cloud.user;
using cloud.lifeCycle;
using Microsoft.EntityFrameworkCore;
using Token = cloud.lifeCycle.Token;

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