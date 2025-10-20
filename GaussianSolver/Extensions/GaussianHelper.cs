using System.Text;

namespace GaussianSolver.Extensions;

public static class GaussianHelper
{
    public static bool IsZero(this double number) => 
        Math.Abs(number) < 1e-10;

    public static double ToPositiveZero(this double number) => 
        number.IsZero() 
            ? Math.Abs(number) 
            : number;

    public static int VarsCount(this double[,] matrix) => 
        matrix.GetLength(1) - 1;
    
    public static int? FindPivotRow(this double[,] matrix, int currentRow, int currentColumn)
    {
        var rank = matrix.VarsCount();
        for (var row = currentRow; row < rank; row++)
            if (!matrix[row, currentColumn].IsZero())
                return row;
        return null;
    }

    public static void SwapRows(this double[,] matrix, int row1, int row2)
    {
        var rank = matrix.VarsCount();
        for (var col = 0; col <= rank; col++)
            (matrix[row1, col], matrix[row2, col]) = (matrix[row2, col], matrix[row1, col]);
    }

    public static void NormalizeRow(this double[,] matrix, int currentRow, int currentColumn)
    {
        var rank = matrix.VarsCount();
        var pivot = matrix[currentRow, currentColumn];
        for (var col = currentColumn; col <= rank; col++)
            matrix[currentRow, col] /= pivot;
    }

    public static void ResetColumnByGaussJordan(this double[,] matrix, int currentRow, int currentColumn)
    {
        var varsCount = matrix.VarsCount();
        var equations = matrix.GetLength(0);
        for (var row = 0; row < equations; row++)
            if (row != currentRow && !matrix[row, currentColumn].IsZero())
            {
                var factor = matrix[row, currentColumn];
                for (var col = currentColumn; col <= varsCount; col++) 
                    matrix[row, col] -= factor * matrix[currentRow, col];
            }
    }
    
    public static bool IsCompatible(this double[,] matrix, int lastPivotRow)
    {
        var equationsCount = matrix.GetLength(0);
        var freeTermIndex = matrix.VarsCount();
        for (var row = lastPivotRow; row < equationsCount; row++)
        {
            var freeTerm = matrix[row, freeTermIndex];
            if (!freeTerm.IsZero())
                return false;
        }
        return true;
    }
    
    public static double[] CalculateSingleSolution(this double[,] matrix)
    {
        var vars = matrix.VarsCount();
        var solution = new double[vars];
        for (var i = 0; i < vars; i++) 
            solution[i] = matrix[i, vars];
        
        return solution;
    }
    
    [Obsolete($"Use {nameof(CalculateSingleSolution)} for reduced echelon matrix")]
    public static double[] CalculateReverseStrokeSolution(this double[,] matrix)
    {
        var vars = matrix.VarsCount();
        var solution = new double[vars];
        for (var i = vars - 1; i >= 0; i--)
        {
            solution[i] = matrix[i, vars];
            for (var j = i + 1; j < vars; j++) 
                solution[i] -= matrix[i, j] * solution[j];
            solution[i] /= matrix[i, i];
        }
        
        return solution;
    }
    
    public static string Print(this double[,] matrix, bool withSeparator = true)
    {
        var sb = new StringBuilder();
        var rows = matrix.GetLength(0);
        var cols = matrix.GetLength(1);
        for (var i = 0; i < rows; i++)
        {
            for (var j = 0; j < cols - 1; j++) 
                sb.Append($"{matrix[i, j],6:F2} ");
            if (withSeparator)
                sb.Append("| ");
            sb.AppendLine($"{matrix[i, cols - 1],6:F2}");
        }
        return sb.ToString();
    }
}