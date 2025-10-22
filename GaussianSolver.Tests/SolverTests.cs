using FluentAssertions;
using GaussianSolver.Models;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;

namespace GaussianSolver.Tests;

[TestFixture]
public class SolverTests
{
    private GaussianSolver gaussianSolver;

    [OneTimeSetUp]
    public void OneTimeSetUp() => 
        gaussianSolver = new GaussianSolver(new NullLogger<GaussianSolver>());

    [TestCaseSource(typeof(ValidTestCases), nameof(ValidTestCases.GetAll))]
    public void Test(double[,] initialMatrix, GaussianSolution expectedSolution)
    {
        var solution = gaussianSolver.Solve(initialMatrix);
        solution.Matrix = null;
        solution.Should().BeEquivalentTo(expectedSolution);
    }
}