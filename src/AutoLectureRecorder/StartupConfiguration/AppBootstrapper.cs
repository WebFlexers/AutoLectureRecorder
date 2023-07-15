using System.ComponentModel;
using Microsoft.Extensions.Hosting;
using ReactiveUI;
using Splat;
using Splat.Microsoft.Extensions.DependencyInjection;
using AutoLectureRecorder.Application;
using AutoLectureRecorder.Infrastructure;
using Microsoft.Extensions.Logging;
using Serilog;
using Splat.ModeDetection;
using Splat.Serilog;

namespace AutoLectureRecorder.StartupConfiguration;

public class AppBootstrapper
{
    public IHost AppHost { get; }

    public AppBootstrapper()
    {
        Splat.ModeDetector.OverrideModeDetector(Mode.Run);
        
        AppHost = Host
            .CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                services.UseMicrosoftDependencyResolver();
                var resolver = Locator.CurrentMutable;
                resolver.InitializeSplat();
                resolver.InitializeReactiveUI();

                // Configure our local services and access the host configuration
                services.AddPresentation()
                    .AddApplication()
                    .AddInfrastructure();
            })
            .ConfigureLogging((hostingContext, builder) =>
            {
                builder.AddLogging();
            })
            .Build();

        // Since MS DI container is a different type,
        // we need to re-register the built container with Splat again
        AppHost.Services.UseMicrosoftDependencyResolver();
    }
}