namespace cloud.firebase;

using Google.Cloud.Firestore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

public class FirestoreBackgroundService : BackgroundService
{
    private readonly ILogger<FirestoreBackgroundService> _logger;
    private FirestoreDb _db;

    public FirestoreBackgroundService(ILogger<FirestoreBackgroundService> logger)
    {
        _logger = logger;
        _db = FirestoreDb.Create("test-firebase-1e6b6");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Firestore Listener Service Started");

        CollectionReference usersRef = _db.Collection("users");
        usersRef.Listen(snapshot =>
        {
            foreach (var docChange in snapshot.Changes)
            {
                switch (docChange.ChangeType)
                {
                    case DocumentChange.Type.Added:
                        _logger.LogInformation($"New User Added: {docChange.Document.Id}");
                        break;
                    case DocumentChange.Type.Modified:
                        _logger.LogInformation($"User Updated: {docChange.Document.Id}");
                        break;
                }
            }
        });

        // Keep the service alive
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Firestore Listener Service Stopping");
        await base.StopAsync(cancellationToken);
    }
}
