using AutoLectureRecorder.Application.Common.Abstractions.Persistence;
using AutoLectureRecorder.Application.Common.Abstractions.WebAutomation;
using AutoLectureRecorder.Infrastructure.Persistence;
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

        services.AddScoped<ISqliteDataAccess, SqliteDataAccess>();
        
        return services;
    }
}