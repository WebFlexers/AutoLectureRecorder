using AutoLectureRecorder.Data.ReactiveModels;
using AutoLectureRecorder.Services.DataAccess.Validation;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Runtime.CompilerServices;

namespace AutoLectureRecorder.DependencyInjection;

internal static class ValidationServicesExtensions
{
    public static IServiceCollection AddFluentValidation(this IServiceCollection services)
    {
        services.AddTransient<IValidator<ReactiveScheduledLecture>, ReactiveScheduledLectureValidator>();

        return services;
    }
}
