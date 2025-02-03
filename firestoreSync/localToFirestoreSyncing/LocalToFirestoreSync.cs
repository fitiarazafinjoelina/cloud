using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using cloud.Database;
using Google.Cloud.Firestore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace cloud.firestoreSync.localToFirestoreSyncing;

public class LocalToFirestoreSync
{
    private readonly FirestoreDb _firestore;
    private readonly List<string> _syncTables;
    private readonly AppDbContext _dbContext;
    public LocalToFirestoreSync(FirestoreDb firestore, IConfiguration configuration,AppDbContext dbContext)
    {
        _firestore = firestore;
        _syncTables = configuration.GetSection("sync:tables").Get<List<string>>();
        _dbContext = dbContext;
    }   
    private Dictionary<string, object> ConvertEntity(object entity)
    {
        var result = new Dictionary<string, object>();
        var formatter = "yyyy-MM-ddTHH:mm:ss.fffZ";

        foreach (var field in entity.GetType().GetProperties())
        {
            var value = field.GetValue(entity);
            if (value is DateTime dateTime)
            {
                value = dateTime.ToString(formatter);
            }
            result[field.Name] = value;
        }
        result["_source"] = "local";
        return result;
    }

    public async Task SyncToFirestoreAsync(object entity, Operation operation)
    {
        try
        {
            Console.WriteLine("Syncing......");
            var tableName = entity.GetType().GetCustomAttribute<TableAttribute>()?.Name ?? "defaultTable";

            if (!_syncTables.Contains(tableName))
            {
                return; 
            }

            var docRef = _firestore.Collection(tableName).Document(GetId(entity));
            var data = ConvertEntity(entity);

            switch (operation)
            {
                case Operation.SaveOrUpdate:
                    await docRef.SetAsync(data);
                    break;

                case Operation.Delete:
                    await docRef.DeleteAsync();
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }

    private string GetId(object entity)
    {
        var entityType = _dbContext.Model.FindEntityType(entity.GetType());
        if (entityType == null)
        {
            throw new Exception($"Entity type {entity.GetType().Name} is not part of the EF Core model.");
        }
        var primaryKey = entityType.FindPrimaryKey();
        if (primaryKey == null || primaryKey.Properties.Count == 0)
        {
            throw new Exception($"No primary key defined for entity type {entity.GetType().Name}.");
        }

        if (primaryKey.Properties.Count > 1)
        {
            throw new Exception($"Composite primary keys are not supported in this implementation for entity type {entity.GetType().Name}.");
        }
        var keyProperty = primaryKey.Properties.First();
        var idValue = keyProperty.PropertyInfo.GetValue(entity);
        if (idValue == null)
        {
            throw new Exception($"The primary key value for {entity.GetType().Name} is null.");
        }

        return idValue.ToString();
    }
}

public enum Operation
{
    SaveOrUpdate,
    Delete
}