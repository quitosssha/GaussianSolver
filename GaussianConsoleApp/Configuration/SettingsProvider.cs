using GaussianConsoleApp.Models;
using Newtonsoft.Json;

namespace GaussianConsoleApp.Configuration;

public static class SettingsProvider
{
    public static async Task<AppSettings> GetSettings()
    {
        var filePath = Path.Combine(GetSolutionPath(), "programSettings.json");
        if (!File.Exists(filePath))
            throw new FileNotFoundException(filePath);
        var json = await File.ReadAllTextAsync(filePath);
        return JsonConvert.DeserializeObject<AppSettings>(json);
    }

    private static string GetSolutionPath()
    {
        var assemblyLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;
        var directory = new DirectoryInfo(Path.GetDirectoryName(assemblyLocation));
        while (directory != null && directory.GetFiles("*.sln").Length == 0)
            directory = directory.Parent;
        return directory?.FullName ?? string.Empty;
    }
}