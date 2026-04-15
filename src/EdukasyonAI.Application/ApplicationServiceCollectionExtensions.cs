using EdukasyonAI.Application.AI;
using EdukasyonAI.Application.Auth;
using EdukasyonAI.Application.Contracts.AI;
using EdukasyonAI.Application.Contracts.Auth;
using EdukasyonAI.Application.Contracts.Courses;
using EdukasyonAI.Application.Contracts.Students;
using EdukasyonAI.Application.Courses;
using EdukasyonAI.Application.Students;
using EdukasyonAI.Domain.Services;
using Microsoft.Extensions.DependencyInjection;

namespace EdukasyonAI.Application;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IStudentAppService, StudentAppService>();
        services.AddScoped<ICourseAppService, CourseAppService>();
        services.AddScoped<IAiAppService, AiAppService>();
        services.AddScoped<IAuthAppService, AuthAppService>();
        services.AddSingleton<AdaptiveLearningService>();
        return services;
    }
}
