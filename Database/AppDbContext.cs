// using cloud.Model;

using cloud.lifeCycle;
using cloud.user;
using cloud.userValidation;
using Microsoft.EntityFrameworkCore;

namespace cloud.Database;

public class AppDbContext: DbContext {
    public DbSet<User> Users { get; set; } // Example DbSet for a 'User' entity
    public DbSet<Token> Tokens { get; set; }
    public DbSet<UserValidation> UserValidations { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
    //     optionsBuilder.UseNpgsql( "Host=localhost;Username=postgres;Password=root;Database=dentiste" );
    // }
    
}