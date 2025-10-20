using GaussianSolver;
using GaussianSolver.Extensions;
using Newtonsoft.Json;

namespace GaussianConsoleApp.Models;

public class Equation
{
    public static Equation Empty(int varsCount) =>
        new Equation { Coefficients = new double[varsCount] };

    [JsonProperty("coefficients", Required = Required.Always)]
    public double[] Coefficients { get; set; }

    [JsonProperty("freeTerm", Required = Required.Always)]
    public double FreeTerm { get; set; }
    
    public bool IsEmpty => Coefficients.All(c => c.IsZero()) && FreeTerm == 0;

    public override string ToString()
    {
        var terms = new List<string>();

        for (var i = 0; i < Coefficients.Length; i++)
        {
            var coef = Coefficients[i];
            var variable = $"x{i + 1}";

            switch (coef)
            {
                case 0:
                    terms.Add($"0{variable}");
                    break;
                case 1:
                    terms.Add($"{variable}");
                    break;
                case -1:
                    terms.Add($"-{variable}");
                    break;
                default:
                    terms.Add($"{coef}{variable}");
                    break;
            }
        }

        return string.Join(" + ", terms).Replace("+ -", "- ") + $" = {FreeTerm}";
    }
}