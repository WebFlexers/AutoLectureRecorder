using System.Diagnostics;
using AutoLectureRecorder.Application.Common.Abstractions.LecturesSchedule;
using AutoLectureRecorder.Application.Common.Abstractions.Persistence;
using AutoLectureRecorder.Application.Common.Abstractions.Recording;
using AutoLectureRecorder.Application.Common.Abstractions.SampleData;
using AutoLectureRecorder.Application.Common.Abstractions.StartupManager;
using AutoLectureRecorder.Application.Common.Abstractions.Validation;
using AutoLectureRecorder.Application.Common.Abstractions.WebAutomation;
using AutoLectureRecorder.Infrastructure.LecturesSchedule;
using AutoLectureRecorder.Infrastructure.Persistence;
using AutoLectureRecorder.Infrastructure.Persistence.Seeding;
using AutoLectureRecorder.Infrastructure.Recording;
using AutoLectureRecorder.Infrastructure.StartupManager;
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

        services.AddSingleton<ILecturesScheduler, LecturesScheduler>();

        services.AddTransient<IStartupManager, WindowsStartupManager>();

        services.AddRepositories();

        if (Debugger.IsAttached)
        {
            services.AddTransient<ISampleData, SampleData>();
        }

        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddSingleton<ISqliteDataAccess, SqliteDataAccess>();
        services.AddSingleton<IScheduledLectureRepository, ScheduledLectureRepository>();
        services.AddSingleton<ISettingsRepository, SettingsRepository>();
        services.AddSingleton<IStudentAccountRepository, StudentAccountRepository>();

        return services;
    }
}