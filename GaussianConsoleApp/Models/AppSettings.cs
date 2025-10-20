using Newtonsoft.Json;

namespace GaussianConsoleApp.Models;

public class AppSettings
{
    [JsonProperty("linearAlgebraicEquationsSystem", Required = Required.Always)]
    public Equation[] Equations { get; set; }

    [JsonProperty("logLevel", NullValueHandling = NullValueHandling.Ignore)]
    public string LogLevel { get; set; }
}