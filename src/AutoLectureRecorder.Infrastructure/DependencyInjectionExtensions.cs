﻿using AutoLectureRecorder.Application.Common.Abstractions.WebAutomation;
using AutoLectureRecorder.Infrastructure.WebAutomation;
using Microsoft.Extensions.DependencyInjection;

namespace AutoLectureRecorder.Infrastructure;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddTransient<IWebDriverDownloader, EdgeWebDriverDownloader>();
        
        return services;
    }
}