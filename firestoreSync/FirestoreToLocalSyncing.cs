using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Reflection;
using cloud.Database;
using Google.Cloud.Firestore;

namespace cloud.firestoreSync;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;

public class EntityClassResolver
{
    private readonly AppDbContext _dbContext;

    public EntityClassResolver(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Type FindEntityClass(string collectionName)
    {
        var model = _dbContext.Model;
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
}

public class FirestoreToLocalSyncing: IHostedService
{
    private readonly List<string> _syncTables;
    private readonly FirestoreDb _firestore;
    private readonly IServiceProvider _serviceProvider; // Inject IServiceProvider
    private readonly ConcurrentDictionary<string, FirestoreChangeListener> _listeners = new ConcurrentDictionary<string, FirestoreChangeListener>();

    
    
    public FirestoreToLocalSyncing(FirestoreDb firestore, IConfiguration configuration, IServiceProvider serviceProvider)
    {
        _firestore = firestore;
        _syncTables = configuration.GetSection("sync:tables").Get<List<string>>();
        _serviceProvider = serviceProvider; // Store the service provider
        Init();
    }
    
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

    private void Init()
    {
        Console.WriteLine("Initializing Firestore to local sync");
        foreach (var table in _syncTables)
        {
            var listener = _firestore.Collection(table)
                .Listen((snapshots, error) =>
                {
                    if (error != null)
                    {
                        Console.Error.WriteLine($"Listen failed: {error}");
                        return null;
                    }
                    foreach (var change in snapshots.Changes)
                    {
                        Console.WriteLine($"Changes detected on table: {table}");
                        HandleDocumentChange(table, change);
                    }
                    return null;
                });

            _listeners[table] = listener;
        }
    }

    public void HandleDocumentChange(string collectionName, DocumentChange change)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var document = change.Document;
            var entityClass = GetEntityClass(collectionName, dbContext);

            if (document.ContainsField("_source") && document.GetValue<string>("_source") == "local")
            {
                return; // Skip local changes
            }

            try
            {
                switch (change.ChangeType)
                {
                    case DocumentChange.Type.Added:
                    case DocumentChange.Type.Modified:
                        SaveOrUpdateEntity(entityClass, document,dbContext);
                        break;

                    case DocumentChange.Type.Removed:
                        DeleteEntity(entityClass, document,dbContext);
                        break;
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine($"Transaction rolled back: {e.Message}");
            }
        }
    }

    private Dictionary<string, object> ConvertEntity(object entity)
    {
        var result = new Dictionary<string, object>();
        var formatter = "yyyy-MM-ddTHH:mm:ss.fffZ"; // ISO 8601 format
        
            foreach (var property in entity.GetType().GetProperties())
            {
                var value = property.GetValue(entity);
                if (value is DateTime dateTime)
                {
                    value = dateTime.ToString(formatter);
                }

                if (Attribute.IsDefined(property, typeof(ColumnAttribute)))
                {
                    var columnName = property.GetCustomAttribute<ColumnAttribute>().Name;
                    result[columnName] = value;
                }
                else
                {
                    result[property.Name] = value;
                }
            
        }

        return result;
    }

    public void SaveOrUpdateEntity(Type entityClass, DocumentSnapshot document,AppDbContext dbContext)
    {
        try
        {
            Console.WriteLine("FROM Firestore");
            long id = long.Parse(document.Id);
            var managedEntity = Activator.CreateInstance(entityClass);
            var data = document.ToDictionary();

            foreach (var property in entityClass.GetProperties())
            {
                if (data.ContainsKey(property.Name))
                {
                    var value = ConvertValue(property.PropertyType, data[property.Name]);
                    property.SetValue(managedEntity, value);
                }
            }

            dbContext.SaveChanges();
            
            // var data2 = ConvertEntity(managedEntity);
            // string sql = PrepareSql(entityClass, data2, id);
            // var parameters = PrepareParams(data2, id);

            // Assuming you have a method to execute raw SQL in your DbContext
            // _dbContext.Database.ExecuteSqlRaw(sql, parameters);

        }
        catch (Exception e)
        {
            Console.Error.WriteLine($"Error saving entity: {e.Message}");
        }
    }


    private void DeleteEntity(Type entityClass, DocumentSnapshot document,AppDbContext dbContext)
    {
        try
        {
            var entity = Activator.CreateInstance(entityClass);
            var idProperty = GetFieldPK(entity);
            
            idProperty.SetValue(entity, long.Parse(document.Id));
            dbContext.Remove(entity); // Use EF Core to remove the entity
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

       return value; // Fallback for other types
   }

   private Type GetEntityClass(string collectionName,AppDbContext dbContext)
   {
       return FindEntityClass(collectionName,dbContext);
   }

   public Task StartAsync(CancellationToken cancellationToken)
   {
       Init();
       return Task.CompletedTask;
   }

   public Task StopAsync(CancellationToken cancellationToken)
   {
       foreach (var listener in _listeners.Values)
       {
           listener.StopAsync();
       }
       return Task.CompletedTask;
   }
}