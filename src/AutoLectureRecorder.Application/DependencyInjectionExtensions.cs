using AutoLectureRecorder.Application.Common.Behaviors;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace AutoLectureRecorder.Application;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(
            typeof(DependencyInjectionExtensions).Assembly));

        services.AddScoped(
            typeof(IPipelineBehavior<,>),
            typeof(ValidationBehavior<,>));
        
        services.AddScoped(
            typeof(IPipelineBehavior<,>),
            typeof(ErrorLoggingBehavior<,>));
        
        services.AddValidatorsFromAssembly(typeof(DependencyInjectionExtensions).Assembly);
        return services;
    }
}