
using cloud.Database;
using Microsoft.EntityFrameworkCore;
using cloud.email;
using cloud.firestoreSync;
using cloud.firestoreSync.localToFirestoreSyncing;
using cloud.lifeCycle;
using cloud.login;
using Microsoft.EntityFrameworkCore;
using cloud.pin;
using cloud.user;
using cloud.userValidation;
using cloud.uniqIdentifier;
using Google.Api.Gax;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
using Grpc.Core;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddHttpClient();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<EntityClassResolver>();
builder.Services.AddScoped<UserValidationService>();
builder.Services.AddScoped<PinService>();
builder.Services.AddScoped<LoginService>();
builder.Services.AddScoped<UniqIndentifierService>();
builder.Services.AddScoped<LocalToFirestoreSync>();
builder.Services.AddScoped<FirestoreSyncListener>();
builder.Services.AddScoped<FirestoreToLocalSyncing>();

builder.Services.AddControllers(); 
builder.Services.Configure<PinSettings>(builder.Configuration.GetSection("PinSettings"));

builder.Services.AddSingleton<PinSettings>(sp =>
    sp.GetRequiredService<IOptions<PinSettings>>().Value);

builder.Services.Configure<TokenSettings>(builder.Configuration.GetSection("TokenSettings"));

builder.Services.AddSingleton<TokenSettings>(sp =>
    sp.GetRequiredService<IOptions<TokenSettings>>().Value
    );

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// builder.Services.AddSingleton<FirestoreToLocalSyncing>(provider =>
// {
//     var dbContextFactory = provider.GetRequiredService<Func<AppDbContext>>(); // Resolves the AppDbContext dynamically
//     var firestore = provider.GetRequiredService<FirestoreDb>();
//     var configuration = provider.GetRequiredService<IConfiguration>();
//     return new FirestoreToLocalSyncing(firestore, configuration, dbContextFactory);
// });


string pathToServiceAccount = "service-account.json";
Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", pathToServiceAccount);
// builder.Services.AddSingleton<FirestoreDb>(provider =>
// {
//     FirestoreClient client = new FirestoreClientBuilder
//     {
//         Endpoint = "localhost:8082",
//         ChannelCredentials = ChannelCredentials.Insecure
//     }.Build();
//     return FirestoreDb.Create("test-firebase-1e6b6", client);
// });
// Console.WriteLine("Created Cloud Firestore client with project ID: {0}", "test-firebase-1e6b6");

builder.Services.AddSingleton<FirestoreDb>(provider =>
{
    return FirestoreConfig.GetFirestoreDbAsync().Result;
});



builder.Services.AddSingleton<FirestoreUniversalListener>(provider => 
    new FirestoreUniversalListener(
        "cloud-syncing",
        "service-account.json",
        provider.GetRequiredService<IConfiguration>(),
        provider.GetRequiredService<IServiceScopeFactory>()
    ));
// builder.Services.AddHostedService<FirestoreToLocalSyncing>();

// Register the factory to resolve AppDbContext
builder.Services.AddScoped<Func<AppDbContext>>(provider => provider.GetRequiredService<AppDbContext>);


// var host = Host.CreateDefaultBuilder(args)
//     .ConfigureServices((context, services) =>
//     {
// var client = await FirestoreConfig.GetFirestoreDbAsync();
// var db = client;
   
   //
   // try
   // {
   //     var docRef = db.Collection("test").Document("debug");
   //     await docRef.SetAsync(new { Test = "Connection Test" }); 
   //     Console.WriteLine("Successfully wrote to emulator!");
   // }
   // catch (Exception ex)
   // {
   //     Console.WriteLine(ex.StackTrace);
   //     Console.WriteLine($"Error: {ex.Message}");
   // }

        // Register the background service
        builder.Services.AddHostedService<FirestoreBackgroundService>();
    // })
    // .Build();

// await host.RunAsync();

// if (builder.Environment.IsDevelopment())
// {
    // Set Firestore emulator URL for local development
    // string emulatorHost = "127.0.0.1:8082"; // Default is 8080
    // Environment.SetEnvironmentVariable("FIRESTORE_EMULATOR_HOST", emulatorHost);
    // Console.WriteLine("Using local Firestore emulator at localhost:8082");
    // }

 
    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            c.RoutePrefix = string.Empty; 
        });
    }

    app.UseAuthorization();

    app.MapControllers();

    var summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    app.MapGet("/weatherforecast", () =>
        {
            var forecast = Enumerable.Range(1, 5).Select(index =>
                    new WeatherForecast
                    (
                        DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                        Random.Shared.Next(-20, 55),
                        summaries[Random.Shared.Next(summaries.Length)]
                    ))
                .ToArray();
            return forecast;
        })
        .WithName("GetWeatherForecast")
        .WithOpenApi();


    app.Run();

    record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
    {
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    }