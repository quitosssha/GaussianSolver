using GaussianConsoleApp.Models;

namespace GaussianConsoleApp;

public static class EquationsSystemValidator
{
    public static void EnsureValidSystem(EquationSystem system)
    {
        var equationsCount = system.Equations.Length;
        if (equationsCount < 1)
            throw new ArgumentException("Система должна содержать хотя бы 1 уравнение!");
        
        if (system.Equations.Any(equation => equation.Coefficients.Length != system.VariablesCount))
            throw new ArgumentException("Все уравнения должны содержать одинаковое количество переменных");
    }
}