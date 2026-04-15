using EdukasyonAI.Infrastructure;
using EdukasyonAI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(cfg =>
    {
        cfg.AddJsonFile("appsettings.json", optional: false);
        cfg.AddEnvironmentVariables();
    })
    .ConfigureServices((ctx, services) =>
    {
        // Use SQLite for local dev migration; override with PostgreSQL via env var
        var useSqlite = ctx.Configuration.GetValue<bool>("UseSqlite", true);
        services.AddInfrastructureServices(ctx.Configuration, useSqlite);
        services.AddLogging(b => b.AddConsole());
    })
    .Build();

using var scope = host.Services.CreateScope();
var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
var db = scope.ServiceProvider.GetRequiredService<EdukasyonDbContext>();

logger.LogInformation("Applying EF Core migrations...");
await db.Database.MigrateAsync();
logger.LogInformation("Migrations applied successfully.");
