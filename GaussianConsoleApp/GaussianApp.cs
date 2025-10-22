using System.Text;
using GaussianConsoleApp.Models;
using GaussianSolver.Extensions;
using GaussianSolver.Models;
using GaussianSolver.Validation;
using Microsoft.Extensions.Logging;

namespace GaussianConsoleApp;

public class GaussianApp
{
    private readonly ILogger logger;
    private readonly GaussianSolver.GaussianSolver gaussianSolver;
    private readonly GaussianValidator gaussianValidator;
    
    public GaussianApp(
        ILogger<GaussianApp> logger, 
        GaussianSolver.GaussianSolver gaussianSolver, 
        GaussianValidator gaussianValidator)
    {
        this.logger = logger;
        this.gaussianSolver = gaussianSolver;
        this.gaussianValidator = gaussianValidator;
    }
    
    public void Run(Equation[] equations)
    {
        var equationSystem = new EquationSystem(equations);
        logger.LogInformation("Исходная система:{line}{system}", Environment.NewLine, equationSystem);
        EquationsSystemValidator.EnsureValidSystem(equationSystem);

        var matrix = equationSystem.ToMatrix();
        var result = gaussianSolver.Solve(matrix);
        var finalSystem = EquationSystem.FromMatrix(result.Matrix);
        logger.LogInformation("Итоговая система:{line}{system}", Environment.NewLine, finalSystem);
        WriteResult(result);
        
        var validationResult = gaussianValidator.Validate(matrix, result);
        logger.Log(
            validationResult.IsValid ? LogLevel.Information : LogLevel.Error, 
            "Результат проверки: {Message}", 
            validationResult.Message);
    }

    private void WriteResult(GaussianSolution result)
    {
        switch (result.SolutionType)
        {
            case SolutionType.None:
                logger.LogInformation("Система не имеет решений.");
                break;
            case SolutionType.Unique:
                logger.LogInformation(
                    "Система имеет единственное решение: {solution}",
                    string.Join(", ", result.Solution.Select((x, i) => $"x{i + 1} = {x}")));
                break;
            case SolutionType.Infinite:
                logger.LogInformation(
                    "Система имеет бесконечное множество решений:{line}{solutions}", 
                    Environment.NewLine,
                    BuildSolutions(result));
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(result.SolutionType));
        }
    }

    private static string BuildSolutions(GaussianSolution result)
    {
        var particularSolution = result.ParticularSolution;
        var basisVectors = result.BasisVectors;
        var equationsCount = result.Matrix.GetLength(0);
        var sb = new StringBuilder();
        
        var numFreeVars = basisVectors.GetLength(0);
        sb.AppendLine();
        sb.AppendLine("Частное решение:");
        for (var i = 0; i < equationsCount; i++)
        {
            sb.AppendLine($"x{i + 1} = {particularSolution[i],8:F4}");
        }
        
        sb.AppendLine();
        sb.AppendLine($"Фундаментальная система решений ({numFreeVars} вектор(а/ов)):");
        for (var i = 0; i < numFreeVars; i++)
        {
            sb.Append($"v{i + 1} = (");
            for (var j = 0; j < equationsCount; j++)
            {
                sb.Append($"{basisVectors[i, j]}");
                if (j < equationsCount - 1) 
                    sb.Append("; ");
            }
            sb.AppendLine(")");
        }
        
        sb.AppendLine();
        sb.AppendLine("Общее решение:");
        sb.Append("x = частное решение");
        for (var i = 0; i < numFreeVars; i++)
        {
            sb.Append($" + c{i + 1}*v{i + 1}");
        }
        sb.AppendLine(", где c1...ck принадлежит R");
        
        sb.AppendLine();
        sb.AppendLine("В развернутом виде:");
        for (var i = 0; i < equationsCount; i++)
        {
            sb.Append($"x{i + 1} = {particularSolution[i],8:F4}");
            for (var j = 0; j < numFreeVars; j++)
            {
                const string space = " ";
                var coeff = basisVectors[j, i];
                if (coeff.IsZero())
                    sb.Append($"   {space,8}   ");
                else
                    sb.Append($" {(coeff < 0 ? "-" : "+")} {Math.Abs(coeff),8:F4}*c{j + 1}");
            }
            sb.AppendLine();
        }
        
        return sb.ToString();
    }
}