using GaussianSolver.Exceptions;
using GaussianSolver.Extensions;
using GaussianSolver.Models;
using Microsoft.Extensions.Logging;

namespace GaussianSolver;

public class GaussianSolver
{
    private readonly ILogger logger;

    public GaussianSolver(ILogger<GaussianSolver> logger)
    {
        this.logger = logger;
    }

    public GaussianSolution Solve(double[,] equationSystemMatrix)
    {
        var extendedMatrix = ExtendAndCloneMatrix(equationSystemMatrix);
        var result = CalculateStraightStroke(extendedMatrix);

        var matrix = result.Matrix;
        var varsCount = matrix.VarsCount();

        if (!matrix.IsCompatible())
            return GaussianSolution.None(matrix);

        if (result.FreeColumns.Count == 0 && result.LastPivotRow >= varsCount)
        {
            if (!matrix.IsReducedEchelonMatrix())
                throw GaussianSolverException.NotReducedEchelonMatrix(matrix);
            
            var solution = matrix.GetFreeTerms();
            return GaussianSolution.Unique(matrix, solution);
        }

        var particularSolution = CalculateParticularSolution(result);
        logger.LogTraceParts("particular solution:", string.Join(", ", particularSolution));
        var basisVectors = CalculateBasisVectors(result);
        return GaussianSolution.Infinite(matrix, particularSolution, basisVectors);
    }

    private static double[,] ExtendAndCloneMatrix(double[,] matrix)
    {
        var varsCount = matrix.VarsCount();
        var equationsCount = matrix.GetLength(0);
        if (equationsCount >= varsCount)
            return (double[,])matrix.Clone();
        
        var lastInitIndex = equationsCount - 1;
        var newMatrix = new double[varsCount, varsCount + 1];
        for (var i = 0; i <= lastInitIndex; i++)
        for (var j = 0; j < varsCount + 1; j++)
            newMatrix[i, j] = matrix[i, j];
        
        return newMatrix;
    }

    private StraightStrokeResult CalculateStraightStroke(double[,] matrix)
    {
        var pivotColumns = new List<int>(); // Базисные переменные
        var freeColumns = new List<int>(); // Свободные переменные

        logger.LogDebugParts("Initialized matrix", matrix.Print());
        logger.LogTraceParts("INIT", matrix.Print());
        var currentRow = 0;
        var rank = matrix.VarsCount();
        for (var col = 0; col < rank; col++)
        {
            logger.LogTraceParts($"row {currentRow} col {col} START", string.Empty);
            var maybePivotRow = matrix.FindPivotRow(currentRow, col);
            if (!maybePivotRow.HasValue)
            {
                freeColumns.Add(col);
                continue;
            }

            var pivotRow = maybePivotRow.Value;
            if (pivotRow != currentRow)
            {
                matrix.SwapRows(pivotRow, currentRow);
                logger.LogTraceParts($"row {currentRow} col {col} SWAP", matrix.Print());
            }

            matrix.NormalizeRow(currentRow, col);
            logger.LogTraceParts($"row {currentRow} col {col} NORMALIZE", matrix.Print());

            matrix.ResetColumnByGaussJordan(currentRow, col);
            logger.LogTraceParts($"row {currentRow} col {col} RESET", matrix.Print());

            logger.LogDebugParts($"Straight stroke [{currentRow}, {col}]", matrix.Print());
            pivotColumns.Add(col);
            currentRow++;
        }

        EnsureAllColumnsProcessedOnce();

        return new StraightStrokeResult
        {
            Matrix = matrix,
            LastPivotRow = currentRow,
            PivotColumns = pivotColumns,
            FreeColumns = freeColumns
        };

        void EnsureAllColumnsProcessedOnce()
        {
            for (var col = 0; col < rank; col++)
                if (pivotColumns.Contains(col) && freeColumns.Contains(col)
                    || !pivotColumns.Contains(col) && !freeColumns.Contains(col))
                    throw GaussianSolverException.WrongPivotAndFreeColumns(col, pivotColumns, freeColumns);
        }
    }

    private static double[] CalculateParticularSolution(StraightStrokeResult result)
    {
        var matrix = result.Matrix;
        var rank = matrix.VarsCount();
        var particularSolution = new double[rank];

        for (var i = 0; i < result.PivotColumns.Count; i++)
        {
            var col = result.PivotColumns[i];
            particularSolution[col] = matrix[i, rank].ToPositiveZero();
        }

        return particularSolution;
    }

    private double[,] CalculateBasisVectors(StraightStrokeResult result)
    {
        var matrix = result.Matrix;
        var varsCount = matrix.VarsCount();
        var pivotColumns = result.PivotColumns;
        var freeColumns = result.FreeColumns;

        logger.LogTraceParts("pivot columns:", string.Join(", ", pivotColumns));
        logger.LogTraceParts("free columns:", string.Join(", ", freeColumns));

        var basisVectors = new double[freeColumns.Count, varsCount];
        for (var i = 0; i < freeColumns.Count; i++)
        {
            var freeVarIndex = freeColumns[i];
            logger.LogTraceParts(
                $"current vector index: {i}, free var index: {freeVarIndex}",
                string.Empty);

            SetFreeVariables(i);
            CalculateBasisCoordinates(i, freeVarIndex);

            logger.LogDebugParts(
                $"Basic Vectors index: {i}, free var index: {freeVarIndex}",
                basisVectors.Print(false));
        }

        return basisVectors;

        void SetFreeVariables(int currentVectorIndex)
        {
            for (var fc = 0; fc < freeColumns.Count; fc++)
            {
                basisVectors[currentVectorIndex, freeColumns[fc]] = currentVectorIndex == fc ? 1 : 0;
                logger.LogTraceParts(
                    $"basis[{currentVectorIndex}, {freeColumns[fc]}] = {basisVectors[currentVectorIndex, freeColumns[fc]]:F2} SET",
                    basisVectors.Print(false));
            }
        }

        void CalculateBasisCoordinates(int currentVectorIndex, int freeVarIndex)
        {
            for (var pc = 0; pc < pivotColumns.Count; pc++)
            {
                var pivotCol = pivotColumns[pc];
                basisVectors[currentVectorIndex, pivotCol] = (-matrix[pc, freeVarIndex]).ToPositiveZero();
                logger.LogTraceParts(
                    $"basis[{currentVectorIndex}, {pivotCol}] = -matrix[{pc}, {freeVarIndex}] " +
                    $"= {basisVectors[currentVectorIndex, pivotCol]:F2} CALCULATE",
                    basisVectors.Print(false));
            }
        }
    }

    private class StraightStrokeResult
    {
        public double[,] Matrix { get; init; }

        public int LastPivotRow { get; init; }

        public List<int> PivotColumns { get; init; }

        public List<int> FreeColumns { get; init; }
    }
}