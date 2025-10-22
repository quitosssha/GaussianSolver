using System.Text;
using GaussianSolver.Extensions;
using GaussianSolver.Models;
using Microsoft.Extensions.Logging;

namespace GaussianSolver.Validation;

public class GaussianValidator
{
    private readonly ILogger logger;

    public GaussianValidator(ILogger<GaussianValidator> logger)
    {
        this.logger = logger;
    }

    public ValidationResult Validate(double[,] originalMatrix, GaussianSolution solution)
    {
        ArgumentNullException.ThrowIfNull(originalMatrix);
        ArgumentNullException.ThrowIfNull(solution);

        return solution.SolutionType switch
        {
            SolutionType.None => ValidationResult.Valid("Нет решений - нечего проверять."),
            SolutionType.Unique => ValidateSolution(originalMatrix, solution.Solution, "Единственное решение"),
            SolutionType.Infinite => ValidateInfiniteSolution(originalMatrix, solution),
            _ => ValidationResult.Invalid($"Неизвестный тип решения: {solution.SolutionType.ToString()}")
        };
    }

    private ValidationResult ValidateInfiniteSolution(double[,] originalMatrix, GaussianSolution solution)
    {
        var particularResult = ValidateSolution(originalMatrix, solution.ParticularSolution, "Частное решение");
        if (!particularResult.IsValid)
            return particularResult;

        if (solution.BasisVectors == null)
            return ValidationResult.Invalid("Бесконечное множество решений без базисных векторов");

        var basisRows = solution.BasisVectors.GetLength(0);
        for (var i = 0; i < basisRows; i++)
        {
            var basisVector = GetRow(solution.BasisVectors, i);
            if (!CheckSatisfiesEquations(originalMatrix, basisVector, true, i))
                return ValidationResult.Invalid($"Базисный вектор {i} не соответствует однородной системе");
        }

        return ValidationResult.Valid("Бесконечное множество решений успешно проверено");
    }

    private ValidationResult ValidateSolution(double[,] matrix, double[] solution, string solutionName)
    {
        if (solution == null)
            return ValidationResult.Invalid($"{solutionName} отсутствует");

        return CheckSatisfiesEquations(matrix, solution)
            ? ValidationResult.Valid($"{solutionName} успешно проверено")
            : ValidationResult.Invalid($"{solutionName} не соответствует изначальной СЛАУ");
    }

    private bool CheckSatisfiesEquations(
        double[,] matrix, 
        double[] solution, 
        bool asHomogeneous = false,
        int? vectorIndex = null)
    {
        var rows = matrix.GetLength(0);
        var varsCount = matrix.VarsCount();

        if (solution.Length != varsCount)
        {
            logger.LogError(
                "Кол-во переменных в уравнении - {varsCount}, а в решении - {solutionLength}",
                varsCount,
                solution.Length);
            return false;
        }

        var systemName = asHomogeneous ? "однородной системы" : "неоднородной системы";
        var validationEntity = asHomogeneous ? "базисном векторе" : "уравнении";
        var sb = new StringBuilder();
        for (var i = 0; i < rows; i++)
        {
            double leftSide = 0;
            for (var j = 0; j < varsCount; j++)
            {
                leftSide += matrix[i, j] * solution[j];

                if (j != 0)
                    sb.Append(" + ");
                var x =
                sb.Append($"{Format(matrix[i, j])}*{Format(solution[j])}");
            }

            var expectedRightSide = asHomogeneous ? 0 : matrix[i, varsCount];
            sb.Append($" = {Format(expectedRightSide)}");
            var resStr = sb.ToString();
            if ((leftSide - expectedRightSide).IsZero())
            {
                var vectorInfo = vectorIndex.HasValue
                    ? $" vector{vectorIndex.Value}"
                    : string.Empty;
                logger.LogDebug("OK{vector} {i}: {validationResult}", vectorInfo, i + 1, resStr);
                sb.Clear();
                continue;
            }

            logger.LogError(
                "Проверка {systemName} не прошла в {i} {validationEntity}: {validationResult}",
                systemName,
                i + 1,
                validationEntity,
                resStr);
            return false;
        }

        return true;
    }

    private static double[] GetRow(double[,] matrix, int rowIndex)
    {
        var cols = matrix.GetLength(1);
        var row = new double[cols];
        for (var j = 0; j < cols; j++)
            row[j] = matrix[rowIndex, j];
        return row;
    }

    private static string Format(double num) =>
        num.IsZero()
            ? "0"
            : num > 0
                ? $"{num:F2}"
                : $"({num:F2})";
}