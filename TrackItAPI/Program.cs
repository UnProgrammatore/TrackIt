using TrackItAPI.Configuration;
using TrackItAPI.Database;
using TrackItAPI.Database.Repositories;
using TrackItAPI.Database.Repositories.Impl;
using TrackItAPI.Database.TypeMappers;
using TrackItAPI.QueueManagement;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var config = builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
if(env is not null)
    config = config.AddJsonFile($"appsettings.{env}.json", optional: true, reloadOnChange: true);
config = config.AddEnvironmentVariables();

builder.Services.Configure<DatabaseConfiguration>(builder.Configuration.GetSection("Database"));
builder.Services.Configure<MQTTConfiguration>(builder.Configuration.GetSection("MQTT"));

builder.Services.AddSingleton<SQLiteVersioning>();
builder.Services.AddSingleton<ITrackingRepository, TrackingRepository>();

builder.Services.AddSingleton<QueueManager>();
builder.Services.AddSingleton<MQTTServer>();

DateTimeTypeMapper.Set();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

await app.Services.GetService<SQLiteVersioning>()!.UpdateDatabaseAsync();
await app.Services.GetService<MQTTServer>()!.StartAsync(CancellationToken.None);
await app.Services.GetService<QueueManager>()!.StartAsync(CancellationToken.None);

app.Run();
