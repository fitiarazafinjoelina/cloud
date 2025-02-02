using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlClient;
using System.Reflection;
using cloud.Database;
using Google.Apis.Auth.OAuth2;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Npgsql;
using NpgsqlTypes;

namespace cloud.firestoreSync;

using FirebaseAdmin;
using Google.Cloud.Firestore;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

public class FirestoreUniversalListener
{
    private readonly FirestoreDb _db;
    private readonly ConcurrentDictionary<string, FirestoreChangeListener> _activeListeners;
    private readonly CancellationTokenSource _cts;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly List<string> _syncTables;

    public FirestoreUniversalListener(string projectId, string credentialsPath, IConfiguration configuration, IServiceScopeFactory serviceScopeFactory)
    {
        FirebaseApp.Create(new AppOptions
        {
            Credential = GoogleCredential.FromFile(credentialsPath)
        });
        _syncTables = configuration.GetSection("sync:tables").Get<List<string>>();
        _db = FirestoreDb.Create(projectId);
        _activeListeners = new ConcurrentDictionary<string, FirestoreChangeListener>();
        _cts = new CancellationTokenSource();
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task StartUniversalListeningAsync()
    {
        // Start with root collections
        await ProcessCollections(await _db.ListRootCollectionsAsync().ToListAsync());
    }

    private async Task ProcessCollections(IEnumerable<CollectionReference> collections)
    {
        foreach (var collection in collections)
        {
            if (_activeListeners.ContainsKey(collection.Path)) continue;

            var listener = collection.Listen(async snapshot =>
            {
                foreach (DocumentChange change in snapshot.Changes)
                {
                    // Console.WriteLine($"Modification on:{change.Document}");
                    HandleDocumentChange(change);
                }
            });

            _activeListeners.TryAdd(collection.Path, listener);

            // Process existing documents in collection
            // await ProcessDocumentsInCollection(collection);
        }
    }

    private async Task ProcessDocumentsInCollection(CollectionReference collection)
    {
        var snapshot = await collection.GetSnapshotAsync();
        foreach (var doc in snapshot.Changes)
        {
            HandleDocumentChange(doc);
        }
    }

    private bool IsLocalChange(DocumentSnapshot document) => 
        document.ContainsField("_source") && 
        document.GetValue<string>("_source") == "local";
    public Type FindEntityClass(string collectionName,AppDbContext dbContext)
    {
        var model = dbContext.Model;
        foreach (var entityType in model.GetEntityTypes())
        {
            var entityClass = entityType.ClrType;
            var tableAttribute = entityClass.GetCustomAttribute<TableAttribute>();

            if (tableAttribute != null && tableAttribute.Name.Equals(collectionName, StringComparison.OrdinalIgnoreCase))
            {
                return entityClass;
            }
        }
        
        throw new InvalidOperationException($"No entity class found for collection: {collectionName}");
    }
    private void HandleDocumentChange(DocumentChange change)
    {
        var collectionId = change.Document.Reference.Parent.Id;
        if (!_syncTables.Contains(collectionId)) return;
        using (var scope = _serviceScopeFactory.CreateScope())
        {
            var _dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            try
            {
                var document = change.Document;
                if (IsLocalChange(document)) return;

                var entityClass = FindEntityClass(collectionId, _dbContext);

                Console.WriteLine("Document Changes:"+change.ChangeType.ToString());
                switch (change.ChangeType)
                {
                    case DocumentChange.Type.Added:
                    case DocumentChange.Type.Modified:
                        SaveOrUpdateEntity(entityClass, document, _dbContext);
                        break;
                    case DocumentChange.Type.Removed:
                        DeleteEntity(entityClass, document, _dbContext);
                        break;
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine($"Error processing {collectionId}: {e.Message}");
            }
        }
    }
    // public void SaveOrUpdateEntity(Type entityClass, DocumentSnapshot document,AppDbContext dbContext)
    // {
    //     try
    //     {
    //         Console.WriteLine("FROM Firestore");
    //         long id = long.Parse(document.Id);
    //         var managedEntity = Activator.CreateInstance(entityClass);
    //         var data = document.ToDictionary();
    //
    //         foreach (var property in entityClass.GetProperties())
    //         {
    //             if (data.ContainsKey(property.Name))
    //             {
    //                 var value = ConvertValue(property.PropertyType, data[property.Name]);
    //                 property.SetValue(managedEntity, value);
    //             }
    //         }
    //         
    //         
    //         
    //         var setMethod = dbContext.GetType().GetMethod("Set");
    //         Console.WriteLine($"{data.ToString()}");
    //
    //         if (setMethod != null)
    //         {
    //             var genericSetMethod = setMethod.MakeGenericMethod(entityClass);
    //             var dbSet = genericSetMethod.Invoke(dbContext, null);  
    //             var findMethod = dbSet.GetType().GetMethod("Find", new[] { typeof(long) });
    //             var dbEntity = findMethod.Invoke(dbSet, new object[] { id });
    //
    //             if (dbEntity != null)
    //             {
    //                 Console.WriteLine($"{dbEntity.ToString()}");
    //                 
    //                 dbContext.Entry(dbEntity).CurrentValues.SetValues(managedEntity);
    //             }
    //             else
    //             {
    //                 dbSet.GetType().GetMethod("Add").Invoke(dbSet, new[] { managedEntity });
    //             }
    //
    //             dbContext.SaveChanges();
    //         }
    //         else
    //         {
    //             Console.WriteLine("Error: Could not find the 'Set' method on the DbContext.");
    //         }
    //         
    //         // var data2 = ConvertEntity(managedEntity);
    //         // string sql = PrepareSql(entityClass, data2, id);
    //         // var parameters = PrepareParams(data2, id);
    //
    //         // Assuming you have a method to execute raw SQL in your DbContext
    //         // _dbContext.Database.ExecuteSqlRaw(sql, parameters);
    //
    //     }
    //     catch (Exception e)
    //     {
    //         Console.WriteLine(e.StackTrace);
    //         Console.Error.WriteLine($"Error saving entity: {e.Message}");
    //     }
    // }
   public void SaveOrUpdateEntity(Type entityClass, DocumentSnapshot document, AppDbContext dbContext)
{
    try
    {
        long id = long.Parse(document.Id);
        var data = document.ToDictionary();

        var entityType = dbContext.Model.FindEntityType(entityClass);
        var tableName = entityType.GetTableName();
        var idColumnName = entityType.FindPrimaryKey().Properties.First().GetColumnName();
        
        bool exists;
        using (var command = dbContext.Database.GetDbConnection().CreateCommand())
        {
            command.CommandText = $"SELECT COUNT(*) FROM {tableName} WHERE {idColumnName} = @p0";
            command.Parameters.Add(new NpgsqlParameter("p0", NpgsqlDbType.Bigint) { Value = id });
            
            dbContext.Database.OpenConnection();
            exists = Convert.ToInt32(command.ExecuteScalar()) > 0;
        }
        var parameters = new List<NpgsqlParameter> { new NpgsqlParameter("p_id", NpgsqlDbType.Bigint) { Value = id } };
        var setClauses = new List<string>();
        var insertColumns = new List<string> { idColumnName };
        var insertValues = new List<string> { "@p_id" };

        foreach (var property in entityClass.GetProperties())
        {
            if (property.GetCustomAttribute<KeyAttribute>() != null) continue;

            var columnAttr = property.GetCustomAttribute<ColumnAttribute>();
            var columnName = columnAttr?.Name ?? property.Name;
            
            if (data.TryGetValue(property.Name, out object value))
            {
                var paramName = $"@p_{columnName}";
                parameters.Add(new NpgsqlParameter(paramName, GetNpgsqlDbType(property.PropertyType))
                {
                    Value = ConvertValue(property.PropertyType, value)
                });

                setClauses.Add($"{columnName} = {paramName}");
                insertColumns.Add(columnName);
                insertValues.Add(paramName);
            }
        }

        if (exists)
        {
            var updateSql = $"UPDATE {tableName} SET {string.Join(", ", setClauses)} WHERE {idColumnName} = @p_id";
            dbContext.Database.ExecuteSqlRaw(updateSql, parameters.ToArray());
        }
        else
        {
            var insertSql = $"INSERT INTO {tableName} ({string.Join(", ", insertColumns)}) " +
                          $"VALUES ({string.Join(", ", insertValues)})";
            dbContext.Database.ExecuteSqlRaw(insertSql, parameters.ToArray());
        }

        dbContext.SaveChanges();
    }
    catch (Exception e)
    {
        Console.Error.WriteLine($"Error saving entity: {e.Message}");
        Console.Error.WriteLine(e.StackTrace);
    }
}

private NpgsqlDbType GetNpgsqlDbType(Type type)
{
    // Handle nullable types
    type = Nullable.GetUnderlyingType(type) ?? type;

    if (type == typeof(int)) return NpgsqlDbType.Integer;
    if (type == typeof(long)) return NpgsqlDbType.Bigint;
    if (type == typeof(string)) return NpgsqlDbType.Text;
    if (type == typeof(DateTime)) return NpgsqlDbType.Timestamp;
    if (type == typeof(bool)) return NpgsqlDbType.Boolean;
    if (type == typeof(decimal)) return NpgsqlDbType.Numeric;
    if (type == typeof(double)) return NpgsqlDbType.Double;
    if (type == typeof(Guid)) return NpgsqlDbType.Uuid;
    
    throw new NotSupportedException($"Type {type.Name} not mapped to NpgsqlDbType");
}

    private void DeleteEntity(Type entityClass, DocumentSnapshot document,AppDbContext dbContext)
    {
        try
        {
            var entity = Activator.CreateInstance(entityClass);
            var idProperty = GetFieldPK(entity);
            
            idProperty.SetValue(entity, long.Parse(document.Id));
            dbContext.Remove(entity);
            dbContext.SaveChanges();
        }
        catch (Exception e)
        {
            Console.Error.WriteLine($"Error deleting entity: {e.Message}");
        }
    }
    private static PropertyInfo GetFieldPK(object entity)
    {
        return entity.GetType().GetProperties()
                   .FirstOrDefault(p => Attribute.IsDefined(p, typeof(KeyAttribute)))
               ?? throw new Exception("No [Key] property found");
    }

    private static object ConvertValue(Type targetType, object value)
    {
        if (value == null) return null;

        if (targetType == typeof(long) || targetType == typeof(long?))
        {
            return Convert.ToInt64(value);
        }
        if (targetType == typeof(int) || targetType == typeof(int?))
        {
            return Convert.ToInt32(value);
        }
        if (targetType == typeof(decimal) || targetType == typeof(decimal?))
        {
            return Convert.ToDecimal(value);
        }
        if (targetType == typeof(double) || targetType == typeof(double?))
        {
            return Convert.ToDouble(value);
        }
        if (targetType == typeof(bool) || targetType == typeof(bool?))
        {
            return Convert.ToBoolean(value);
        }
        if (targetType == typeof(string))
        {
            return value.ToString();
        }
        if (targetType == typeof(DateTime) || targetType == typeof(DateTime?))
        {
            if (value is string dateString)
            {
                if (DateTime.TryParse(dateString, out var date))
                {
                    return date;
                }
                else
                {
                    throw new InvalidCastException($"Unable to convert the string '{dateString}' to a DateTime.");
                }
            }
            if (value is DateTime)
            {
                return (DateTime)value;
            }
            if (value is DateTime?)
            {
                return (DateTime?)value;
            }
        }
        return value; 
    }

    public async Task StopAllListenersAsync()
    {
        _cts.Cancel();
        foreach (var listener in _activeListeners.Values)
        {
            await listener.StopAsync();
        }
        _activeListeners.Clear();
    }

    // // Example usage
    // public static async Task Demo()
    // {
    //     var listener = new FirestoreUniversalListener("your-project", "service-account.json");
    //     await listener.StartUniversalListeningAsync();
    //     Console.WriteLine("Listening to all Firestore changes. Press any key to exit...");
    //     Console.ReadKey();
    //     await listener.StopAllListenersAsync();
    // }
}