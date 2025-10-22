using FluentAssertions;
using GaussianSolver.Extensions;
using GaussianSolver.Models;
using GaussianSolver.Validation;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;

namespace GaussianSolver.Tests;

[TestFixture]
public class ValidatorTests
{
    private GaussianValidator gaussianValidator;

    [OneTimeSetUp]
    public void OneTimeSetup() => 
        gaussianValidator = new GaussianValidator(new NullLogger<GaussianValidator>());

    [TestCaseSource(typeof(ValidTestCases), nameof(ValidTestCases.GetAll))]
    public void ValidTests(double[,] originalMatrix, GaussianSolution solution)
    {
        var validationResult = gaussianValidator.Validate(originalMatrix, solution);
        validationResult.IsValid.Should().BeTrue();
    }

    [TestCaseSource(typeof(ValidTestCases), nameof(ValidTestCases.GetUnique))]
    [TestCaseSource(typeof(ValidTestCases), nameof(ValidTestCases.GetInfinite))]
    public void InvalidByModifiedMatrixTests(double[,] originalMatrix, GaussianSolution solution)
    {
        originalMatrix[0, 0]++;
        AssertInvalid(originalMatrix, solution);
    }

    [TestCaseSource(typeof(ValidTestCases), nameof(ValidTestCases.GetUnique))]
    public void InvalidSolutionTests(double[,] originalMatrix, GaussianSolution solution)
    {
        solution.Solution[0]++;
        AssertInvalid(originalMatrix, solution);
    }

    [TestCaseSource(typeof(ValidTestCases), nameof(ValidTestCases.GetInfinite))]
    public void InvalidParticularSolutionTests(double[,] originalMatrix, GaussianSolution solution)
    {
        if (IsZeroMatrix(originalMatrix))
            Assert.Ignore("Any particular solution should be valid");
        solution.ParticularSolution[0]++;
        AssertInvalid(originalMatrix, solution);
    }

    [TestCaseSource(typeof(ValidTestCases), nameof(ValidTestCases.GetInfinite))]
    public void InvalidBasisVectorsTests(double[,] originalMatrix, GaussianSolution solution)
    {
        if (IsZeroMatrix(originalMatrix))
            Assert.Ignore("Any basis vectors should be valid");
        solution.BasisVectors[0, 0]++;
        AssertInvalid(originalMatrix, solution);
    }

    private void AssertInvalid(double[,] originalMatrix, GaussianSolution solution)
    {
        var validationResult = gaussianValidator.Validate(originalMatrix, solution);
        if (validationResult.IsValid)
            Console.WriteLine(validationResult.Message);
        validationResult.IsValid.Should().BeFalse();
    }

    private static bool IsZeroMatrix(double[,] matrix)
    {
        for (var i = 0; i < matrix.GetLength(0); i++)
        for (var j = 0; j < matrix.GetLength(1); j++)
            if (!matrix[i, j].IsZero())
                return false;
        return true;
    }
}