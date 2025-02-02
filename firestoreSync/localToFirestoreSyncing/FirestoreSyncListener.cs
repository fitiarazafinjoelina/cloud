namespace cloud.firestoreSync.localToFirestoreSyncing;


public class FirestoreSyncListener
{
    private readonly LocalToFirestoreSync _firestoreSyncService;

    public FirestoreSyncListener(LocalToFirestoreSync firestoreSyncService)
    {
        _firestoreSyncService = firestoreSyncService;
    }

    public async Task PostPersistOrUpdateAsync(object entity)
    {
        await _firestoreSyncService.SyncToFirestoreAsync(entity, Operation.SaveOrUpdate);
    }

    public async Task PostRemoveAsync(object entity)
    {
        await _firestoreSyncService.SyncToFirestoreAsync(entity, Operation.Delete);
    }
}