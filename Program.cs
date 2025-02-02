
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
using Google.Cloud.Firestore;
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
// builder.Services.AddHostedService<FirestoreToLocalSyncing>();

// Register the factory to resolve AppDbContext
builder.Services.AddScoped<Func<AppDbContext>>(provider => provider.GetRequiredService<AppDbContext>);


// var host = Host.CreateDefaultBuilder(args)
//     .ConfigureServices((context, services) =>
//     {
   builder.Services.AddSingleton<FirestoreUniversalListener>(provider => 
     new FirestoreUniversalListener(
         "test-firebase-1e6b6",
         "service-account.json",
         provider.GetRequiredService<IConfiguration>(),
         provider.GetRequiredService<IServiceScopeFactory>()
     ));


        // Register the background service
        builder.Services.AddHostedService<FirestoreBackgroundService>();
    // })
    // .Build();

// await host.RunAsync();

string pathToServiceAccount = "service-account.json";
Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", pathToServiceAccount);
builder.Services.AddSingleton<FirestoreDb>(provider => FirestoreDb.Create("test-firebase-1e6b6"));
Console.WriteLine("Created Cloud Firestore client with project ID: {0}", "test-firebase-1e6b6");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        c.RoutePrefix = string.Empty; // Set this to '/' if Swagger is the homepage
    });
}

// app.UseHttpsRedirection();
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