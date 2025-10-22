using FluentAssertions;
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
    [TestCaseSource(typeof(ValidTestCases), nameof(ValidTestCases.GetInfinite), [ true ])]
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

    [TestCaseSource(typeof(ValidTestCases), nameof(ValidTestCases.GetInfinite), [ false ])]
    public void InvalidParticularSolutionTests(double[,] originalMatrix, GaussianSolution solution)
    {
        solution.ParticularSolution[0]++;
        AssertInvalid(originalMatrix, solution);
    }

    [TestCaseSource(typeof(ValidTestCases), nameof(ValidTestCases.GetInfinite), [ false ])]
    public void InvalidBasisVectorsTests(double[,] originalMatrix, GaussianSolution solution)
    {
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
}