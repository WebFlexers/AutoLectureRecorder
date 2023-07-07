using Microsoft.Extensions.DependencyInjection;

namespace AutoLectureRecorder.Application;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        return services;
    }
}