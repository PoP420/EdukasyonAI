using EdukasyonAI.Application.AI;
using EdukasyonAI.Domain.Entities;
using EdukasyonAI.Domain.Repositories;
using EdukasyonAI.Infrastructure.AI;
using EdukasyonAI.Infrastructure.Data;
using EdukasyonAI.Infrastructure.Repositories;
using EdukasyonAI.Infrastructure.Sync;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EdukasyonAI.Infrastructure;

public static class InfrastructureServiceCollectionExtensions
{
    /// <summary>
    /// Registers EF Core with SQLite (for offline / MAUI) or PostgreSQL (for cloud host).
    /// </summary>
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration,
        bool useSqlite = false)
    {
        if (useSqlite)
        {
            services.AddDbContext<EdukasyonDbContext>(opts =>
                opts.UseSqlite(configuration.GetConnectionString("SQLite") ?? "Data Source=edukasyon.db"));
        }
        else
        {
            services.AddDbContext<EdukasyonDbContext>(opts =>
                opts.UseNpgsql(configuration.GetConnectionString("PostgreSQL")));
        }

        // Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IStudentProfileRepository, StudentProfileRepository>();
        services.AddScoped<IStudentProgressRepository, StudentProgressRepository>();
        services.AddScoped<ICourseRepository, CourseRepository>();
        services.AddScoped<IPracticeQuestionRepository, PracticeQuestionRepository>();
        services.AddScoped<IRepository<Lesson>, EfRepository<Lesson>>();
        services.AddScoped<IRepository<PracticeSession>, EfRepository<PracticeSession>>();
        services.AddScoped<IRepository<SessionAnswer>, EfRepository<SessionAnswer>>();
        services.AddScoped<IRepository<AuditLog>, EfRepository<AuditLog>>();

        // AI
        services.AddHttpClient<INemotronService, NemotronService>();

        // Offline sync
        services.AddScoped<OfflineSyncService>();

        return services;
    }
}
