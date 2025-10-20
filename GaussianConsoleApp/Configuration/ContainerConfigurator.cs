using System.Reflection;
using Autofac;
using GaussianConsoleApp.Models;
using GaussianSolver.AutofacModule;
using Microsoft.Extensions.Logging;

namespace GaussianConsoleApp.Configuration;

public static class ContainerConfigurator
{
    public static IContainer Configure(AppSettings appSettings)
    {
        var logLevel = GetLogLevel(appSettings.LogLevel);
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole().SetMinimumLevel(logLevel));

        var builder = new ContainerBuilder();
        
        builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
            .AsSelf()
            .AsImplementedInterfaces()
            .InstancePerLifetimeScope();
        
        builder.RegisterInstance(loggerFactory).As<ILoggerFactory>().SingleInstance();
        builder.RegisterGeneric(typeof(Logger<>)).As(typeof(ILogger<>)).SingleInstance();

        builder.RegisterModule<SolverModule>();
        
        return builder.Build();
    }

    private static LogLevel GetLogLevel(string logLevel) =>
        logLevel switch
        {
            "trace" => LogLevel.Trace,
            "debug" => LogLevel.Debug,
            _ => LogLevel.Information
        };
}