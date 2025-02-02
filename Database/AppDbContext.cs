using cloud.firestoreSync.localToFirestoreSyncing;
using cloud.user;
using cloud.lifeCycle;
using cloud.user;
using cloud.pin;
using cloud.temporaryToken;
using cloud.uniqIdentifier;
using cloud.userValidation;
using Microsoft.EntityFrameworkCore;
using Token = cloud.lifeCycle.Token;

namespace cloud.Database;

public class AppDbContext: DbContext {
    public DbSet<User> Users { get; set; } // Example DbSet for a 'User' entity
    public DbSet<Token> Tokens { get; set; }
    public DbSet<UserValidation> UserValidations { get; set; }
    public DbSet<TemporaryToken> TemporaryTokens { get; set; }
    public DbSet<Pin> Pins { get; set; }
    
    public DbSet<UniqIdentifier> UniqIdentifiers { get; set; }
    private readonly IServiceProvider _serviceProvider;
    public AppDbContext(DbContextOptions<AppDbContext> options, IServiceProvider serviceProvider)
        : base(options)
    {
        _serviceProvider = serviceProvider;
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var syncEntries = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted)
            .ToList();
        var result = 0;
        Console.WriteLine("----------------------- SaveChangesAsync --------------------------");
        var firestoreSyncListener = _serviceProvider.GetRequiredService<FirestoreSyncListener>();
        
        foreach (var entry in syncEntries)
        {            
            var state = entry.State;
            result = await base.SaveChangesAsync(cancellationToken);
            Console.WriteLine(entry.State);
            if (state == EntityState.Added || state == EntityState.Modified)
            {
                Console.WriteLine("Adding to firestore: " + entry);
                await firestoreSyncListener.PostPersistOrUpdateAsync(entry.Entity);
            }
            else if (state == EntityState.Deleted)
            {
                await firestoreSyncListener.PostRemoveAsync(entry.Entity);
            }
        }

        return result;
    }
    public override int SaveChanges()
    {
        var syncEntries = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted)
            .ToList();

        Console.WriteLine("----------------------- SaveChanges --------------------------");

        var firestoreSyncListener = _serviceProvider.GetRequiredService<FirestoreSyncListener>();
        var result = 0;
        
        foreach (var entry in syncEntries)
        {   
            var state = entry.State;
            result = base.SaveChanges();
            Console.WriteLine(entry.State);
            if (state == EntityState.Added || state == EntityState.Modified)
            {
                Console.WriteLine("Adding to firestore: " + entry);
                firestoreSyncListener.PostPersistOrUpdateAsync(entry.Entity).GetAwaiter().GetResult();
            }
            else if (state == EntityState.Deleted)
            {
                firestoreSyncListener.PostRemoveAsync(entry.Entity).GetAwaiter().GetResult();
            }
        }
        return result;
    }


    // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
    //     optionsBuilder.UseNpgsql( "Host=localhost;Username=postgres;Password=root;Database=dentiste" );
    // }
    
}