using ValidationAPI.Models;
using ValidationAPI.Repositories;
using ValidationAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpClient();
builder.Services.Configure<MongoSettings>(
    builder.Configuration.GetSection("MongoSettings"));

builder.Services.Configure<Notification>(
    builder.Configuration.GetSection("Notification"));

// Configure biometry settings
builder.Configuration.GetSection("AllowedFileTypes").Bind(new List<string>());
builder.Configuration.GetSection("MaxFileSizeMB").Bind(new int());

builder.Services.Configure<MongoSettings>(settings =>
{
    settings.ConnectionString = "mongodb://mongodb:27017";
    settings.DatabaseName = "ValidationDB";
});

builder.Services.AddScoped<ValidationRepository>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Register services
builder.Services.AddScoped<MetadataService>();
builder.Services.AddScoped<ValidationRepository>();
builder.Services.AddScoped<NotificationService>();

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(5000);
});

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
