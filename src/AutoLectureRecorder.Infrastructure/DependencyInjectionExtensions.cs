using AutoLectureRecorder.Application.Common.Abstractions.Persistence;
using AutoLectureRecorder.Application.Common.Abstractions.Recording;
using AutoLectureRecorder.Application.Common.Abstractions.Validation;
using AutoLectureRecorder.Application.Common.Abstractions.WebAutomation;
using AutoLectureRecorder.Infrastructure.Persistence;
using AutoLectureRecorder.Infrastructure.Recording;
using AutoLectureRecorder.Infrastructure.Validation;
using AutoLectureRecorder.Infrastructure.WebAutomation;
using Microsoft.Extensions.DependencyInjection;

namespace AutoLectureRecorder.Infrastructure;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddTransient<IWebDriverDownloader, EdgeWebDriverDownloader>();
        services.AddTransient<IAlrWebDriver, UnipiEdgeWebDriver>();
        services.AddTransient<IWebDriverFactory, WebDriverFactory>();

        services.AddTransient<IRecorder, WindowsRecorder>();

        services.AddSingleton<IPersistentValidationContext, PersistentValidationContext>();

        services.AddRepositories();

        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<ISqliteDataAccess, SqliteDataAccess>();
        services.AddScoped<IScheduledLectureRepository, ScheduledLectureRepository>();
        services.AddScoped<ISettingsRepository, SettingsRepository>();
        services.AddScoped<IStudentAccountRepository, StudentAccountRepository>();

        return services;
    }
}