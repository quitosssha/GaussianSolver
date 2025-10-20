namespace GaussianConsoleApp.Models;

public class EquationSystem
{
    public EquationSystem(Equation[] equations)
    {
        VariablesCount = equations.FirstOrDefault()?.Coefficients.Length ?? 0;
        var diff = VariablesCount - equations.Length;
        if (diff < 0)
            diff = 0;
        var newEquations = equations.Concat(Enumerable.Repeat(Equation.Empty(VariablesCount), diff)).ToArray();
        Equations = newEquations;
    }

    public int VariablesCount { get; }

    public Equation[] Equations { get; }

    public override string ToString() =>
        string.Join(
            Environment.NewLine,
            Equations
                .Where(e => !e.IsEmpty)
                .Select(e => e.ToString()));

    public double[,] ToMatrix()
    {
        var matrix = new double[Equations.Length, VariablesCount + 1];
        for (var i = 0; i < Equations.Length; i++)
        {
            var equation = Equations[i];
            for (var j = 0; j < VariablesCount; j++)
                matrix[i, j] = equation.Coefficients[j];
            matrix[i, VariablesCount] = equation.FreeTerm;
        }

        return matrix;
    }

    public static EquationSystem FromMatrix(double[,] matrix)
    {
        var height = matrix.GetLength(0);
        var width = matrix.GetLength(1);
        var equations = new Equation[height];
        for (var i = 0; i < height; i++)
        for (var j = 0; j < width; j++)
        {
            equations[i] ??= new Equation { Coefficients = new double[width - 1] };
            if (j < width - 1)
                equations[i].Coefficients[j] = matrix[i, j];
            else
                equations[i].FreeTerm = matrix[i, j];
        }

        return new EquationSystem(equations);
    }
}