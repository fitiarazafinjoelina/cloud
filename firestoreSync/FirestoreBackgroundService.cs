namespace cloud.firestoreSync;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

public class FirestoreBackgroundService : BackgroundService
{
    private readonly FirestoreUniversalListener _firestoreListener;
    private readonly ILogger<FirestoreBackgroundService> _logger;

    public FirestoreBackgroundService(
        FirestoreUniversalListener firestoreListener,
        ILogger<FirestoreBackgroundService> logger)
    {
        _firestoreListener = firestoreListener;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting Firestore background listener...");
        
        try
        {
            // Start listening to all collections
            await _firestoreListener.StartUniversalListeningAsync();
            
            // Keep the service running until cancelled
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
        catch (TaskCanceledException)
        {
            // Graceful shutdown
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Firestore background service failed");
        }
        finally
        {
            await _firestoreListener.StopAllListenersAsync();
            _logger.LogInformation("Firestore background service stopped");
        }
    }
}