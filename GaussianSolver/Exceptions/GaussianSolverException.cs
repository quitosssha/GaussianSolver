using GaussianSolver.Extensions;
using static System.Environment;

namespace GaussianSolver.Exceptions;

public class GaussianSolverException : Exception
{
    private GaussianSolverException(string message) : base("Ошибка алгоритма: " + message)
    {
    }

    public static GaussianSolverException WrongPivotAndFreeColumns(
        int col,
        List<int> pivotColumns,
        List<int> freeColumns) =>
        new($"Переменная {col} не распределена или пересекается между базисными и свободными переменными." + NewLine +
            $"Базисные: [{string.Join(", ", pivotColumns)}]" + NewLine +
            $"Свободные: [{string.Join(", ", freeColumns)}]");

    public static GaussianSolverException NotReducedEchelonMatrix(double[,] matrix) =>
        new("Матрица не приведена к редуцированному ступенчатому виду!" + NewLine + matrix.Print());
}