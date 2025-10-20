using Autofac;
using GaussianConsoleApp.Configuration;
using Microsoft.Extensions.Logging;

namespace GaussianConsoleApp;

public class Program
{
    private static IContainer container;
    
    public static async Task Main()
    {
        try
        {
            var settings = await SettingsProvider.GetSettings();
            container = ContainerConfigurator.Configure(settings);
            container.Resolve<GaussianApp>().Run(settings.Equations);
        }
        catch (Exception ex)
        {
            if (container == null)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine(ex.ToString());
                Console.ResetColor();
                return;
            }
            container.Resolve<ILogger<Program>>().LogError(ex, "Ошибка выполнения.");
        }
    }
}
