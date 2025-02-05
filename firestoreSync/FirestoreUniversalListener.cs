using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlClient;
using System.Reflection;
using cloud.Database;
using cloud.email;
using cloud.helper;
using cloud.user;
using cloud.userValidation;
using Google.Apis.Auth.OAuth2;
using Google.Type;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Npgsql;
using NpgsqlTypes;

namespace cloud.firestoreSync;
using AutoMapper;
using FirebaseAdmin;
using Google.Cloud.Firestore;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
public class SaveOrUpdateContext
{
    public Type EntityClass { get; set; }
    public DocumentSnapshot Document { get; set; }
    public AppDbContext DbContext { get; set; }
    public string TableName { get; set; }
    public string IdColumnName { get; set; }
    public int Id { get; set; }
    public Dictionary<string, object> Data { get; set; }
}

public class FirestoreUniversalListener
{
    private readonly FirestoreDb _db;
    private readonly ConcurrentDictionary<string, FirestoreChangeListener> _activeListeners;
    private readonly CancellationTokenSource _cts;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly List<string> _syncTables;
    private EmailService _emailService;
    private UserService _userService;
    public FirestoreUniversalListener(string projectId, string credentialsPath, IConfiguration configuration, IServiceScopeFactory serviceScopeFactory)
    {
        FirebaseApp.Create(new AppOptions
        {
            ProjectId = projectId,
            Credential = GoogleCredential.FromFile(credentialsPath)
        });
        _syncTables = configuration.GetSection("sync:tables").Get<List<string>>();
        _db = FirestoreConfig.GetFirestoreDbAsync().Result;
        _db = FirestoreDb.Create(projectId);
        _activeListeners = new ConcurrentDictionary<string, FirestoreChangeListener>();
        _cts = new CancellationTokenSource();
        _serviceScopeFactory = serviceScopeFactory;
        _emailService = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<EmailService>();
        _userService = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<UserService>();
        _userService.Hash = false;
    }

    
    // private readonly FirestoreDb _db;
    // private readonly ConcurrentDictionary<string, FirestoreChangeListener> _activeListeners;
    // private readonly CancellationTokenSource _cts;
    // private readonly IServiceScopeFactory _serviceScopeFactory;
    // private readonly List<string> _syncTables;
    //
    // private EmailService _emailService;
    // private UserService _userService;
    // public FirestoreUniversalListener(string projectId, string credentialsPath, IConfiguration configuration, IServiceScopeFactory serviceScopeFactory)
    // {
    //     FirebaseApp.Create(new AppOptions
    //     {
    //         Credential = GoogleCredential.FromFile(credentialsPath)
    //     });
    //     _syncTables = configuration.GetSection("sync:tables").Get<List<string>>();
    //     _db = FirestoreDb.Create(projectId);
    //     _activeListeners = new ConcurrentDictionary<string, FirestoreChangeListener>();
    //     _cts = new CancellationTokenSource();
    //     _serviceScopeFactory = serviceScopeFactory;
    //     _emailService = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<EmailService>();
    //     _userService = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<UserService>();
    //     _userService.Hash = false;
    // }
    public async Task StartUniversalListeningAsync()
    {
        // Start with root collections
        await ProcessCollections(await _db.ListRootCollectionsAsync().ToListAsync());
    }

    private async Task ProcessCollections(IEnumerable<CollectionReference> collections)
    {
        foreach (var collection in collections)
        {
            Console.WriteLine("Processing collection: " + collection.Path);

            if (_activeListeners.ContainsKey(collection.Path))
                continue;

            try
            {
                var listener = collection.Listen(async snapshot =>
                {
                    foreach (DocumentChange change in snapshot.Changes)
                    {
                        Console.WriteLine($"Modification on:{change.Document}");
                        HandleDocumentChange(change);
                    }
                });

                _activeListeners.TryAdd(collection.Path, listener);

                Console.WriteLine("Listener added for collection: " + collection.Path);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error adding listener for {collection.Path}: {ex.Message}");
            }

            // Process existing documents in collection
            await ProcessDocumentsInCollection(collection);
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
                var context = new SaveOrUpdateContext
                {
                    Document = document,
                    DbContext = _dbContext,
                    TableName = null,
                    IdColumnName = null,
                    Data = document.ToDictionary(),
                    EntityClass = FindEntityClass(collectionId, _dbContext)
                };

                switch (change.ChangeType)
                {
                    case DocumentChange.Type.Added:
                    case DocumentChange.Type.Modified:
                        SaveOrUpdateEntity(context);
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
    
   public void SaveOrUpdateEntity(SaveOrUpdateContext context)
   {
       try
       {
           try
           {
               context.Id = Convert.ToInt32(context.Document.Id);
           }
           catch (Exception e)
           {
               
           }
           context.Data = context.Document.ToDictionary();

           var entityType = context.DbContext.Model.FindEntityType(context.EntityClass);
           context.TableName = entityType.GetTableName();
           context.IdColumnName = GetPrimaryKeyColumnName(context);
           
           Console.WriteLine("Primary key field is:"+context.IdColumnName);
           if (context.TableName.CompareTo("user_validation") == 0)
           {
               HandleUserValidation(context);
               return;
           }
           var primaryKeyProperty = entityType.FindPrimaryKey()?.Properties.FirstOrDefault();
           var primaryKeyValue = context.Document.ToDictionary().GetValueOrDefault(primaryKeyProperty?.Name);
           if (primaryKeyValue != null)
           {               
               Console.WriteLine($"ID value: {context.Id}");
               context.Id = Convert.ToInt32(primaryKeyValue);
               Console.WriteLine($"Converted ID: {context.Id}");
           }
           else
           {
               Console.WriteLine("Primary key value is missing or null in the document.");
               context.Id = 0;
           }
           foreach (var property in context.Document.ToDictionary())
           {
               
               Console.WriteLine($"{property.Key}: {property.Value}");
           }
           Console.WriteLine("ID:" + context.Id);

           bool exists = context.Id != 0 && EntityExists(context); 

           var parameters = new List<NpgsqlParameter> { new NpgsqlParameter("p_id", NpgsqlDbType.Bigint) { Value = context.Id } };
           var setClauses = new List<string>();
           var insertColumns = new List<string> { context.IdColumnName };
           var insertValues = new List<string> { "@p_id" };

           PrepareEntityParameters(context, parameters, setClauses, insertColumns, insertValues);

           if (exists)
           {
               UpdateEntity(context, setClauses, parameters);
           }
           else
           {
               InsertEntity(context, insertColumns, insertValues, parameters);
           }
           context.DbContext.SaveChanges();
       }
       catch (Exception e)
       {
           Console.Error.WriteLine($"Error saving entity: {e.Message}");
           Console.Error.WriteLine(e.StackTrace);
       }
   }
   private void HandleUserValidation(SaveOrUpdateContext context)
   {
       var configuration = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
       var mapper = new Mapper(configuration);

       UserInscriptionDTO person = mapper.Map<Dictionary<string, object>, UserInscriptionDTO>(context.Data);
       UserValidation userValidation = _userService.SignUpUser(person);
       _emailService.SendEmailAsync("Cloud", person.Email, "Confirmation Compte", EmailHelper.GetValidationEmail(userValidation.Id));
   }
   private string GetPrimaryKeyColumnName(SaveOrUpdateContext context)
   {
       var entityType = context.DbContext.Model.FindEntityType(context.EntityClass);
       return entityType.FindPrimaryKey().Properties.First().GetColumnName();
   }
   private bool EntityExists(SaveOrUpdateContext context)
   {
       using (var command = context.DbContext.Database.GetDbConnection().CreateCommand())
       {
           command.CommandText = $"SELECT COUNT(*) FROM {context.TableName} WHERE {context.IdColumnName} = @p0";
           command.Parameters.Add(new NpgsqlParameter("p0", NpgsqlDbType.Bigint) { Value = context.Id });

           context.DbContext.Database.OpenConnection();
           return Convert.ToInt32(command.ExecuteScalar()) > 0;
       }
   }
   private void PrepareEntityParameters(SaveOrUpdateContext context, List<NpgsqlParameter> parameters,
       List<string> setClauses, List<string> insertColumns, List<string> insertValues)
   {
       foreach (var property in context.EntityClass.GetProperties())
       {
           if (property.GetCustomAttribute<KeyAttribute>() != null) continue;

           var columnAttr = property.GetCustomAttribute<ColumnAttribute>();
           var columnName = columnAttr?.Name ?? property.Name;

           if (context.Data.TryGetValue(property.Name, out object value))
           {
               if (value == null)
               {
                   value = DBNull.Value;
               }
               
               if (value is Google.Cloud.Firestore.Timestamp timestamp)
               {
                   value = timestamp.ToDateTime().ToUniversalTime();
               }
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
   }
   private void UpdateEntity(SaveOrUpdateContext context, List<string> setClauses, List<NpgsqlParameter> parameters)
   {
       var updateSql = $"UPDATE {context.TableName} SET {string.Join(", ", setClauses)} WHERE {context.IdColumnName} = @p_id";
       context.DbContext.Database.ExecuteSqlRaw(updateSql, parameters.ToArray());
   }
   private void InsertEntity(SaveOrUpdateContext context, List<string> insertColumns, List<string> insertValues,
       List<NpgsqlParameter> parameters)
   {
       if (context.Id == 0)
       {
           insertColumns.Remove(context.IdColumnName); 
           insertValues.Remove($"@p_id");
           Console.WriteLine("ID is 0, allowing the database to auto-generate the key.");
       }
       var insertSql = $"INSERT INTO {context.TableName} ({string.Join(", ", insertColumns)}) " +
                       $"VALUES ({string.Join(", ", insertValues)})";
       context.DbContext.Database.ExecuteSqlRaw(insertSql, parameters.ToArray());
   }

    private NpgsqlDbType GetNpgsqlDbType(Type type)
    {
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
            // idProperty.SetValue(entity, Convert.ToInt32(document.ToDictionary()[idProperty.Name]));
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
            Console.WriteLine( value.GetType().ToString());
            if (value is string dateString)
            {
                Console.WriteLine($"Est date string:{dateString}");
                if (DateTime.TryParse(dateString, out var date))
                {
                    Console.WriteLine(date);
                    date = (DateTime.SpecifyKind((DateTime) value,DateTimeKind.Local));
                    return date;
                }
                else
                {
                    throw new InvalidCastException($"Unable to convert the string '{dateString}' to a DateTime.");
                }
            }
            if (value is DateTime)
            {
                value = (DateTime.SpecifyKind((DateTime) value,DateTimeKind.Local));
                return (DateTime)value;
            }
            if (value is DateTime?)
            {
                value = (DateTime.SpecifyKind((DateTime) value,DateTimeKind.Local));
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
}