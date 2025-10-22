using GaussianSolver.Extensions;

namespace GaussianSolver.Models;

public class GaussianSolution : IEquatable<GaussianSolution>
{
    public static GaussianSolution None(double[,] matrix) =>
        new(SolutionType.None, matrix);

    public static GaussianSolution Unique(double[,] matrix, double[] solution) =>
        new(SolutionType.Unique, matrix, solution);

    public static GaussianSolution Infinite(double[,] matrix, double[] particularSolution, double[,] basisVectors) =>
        new(SolutionType.Infinite, matrix, particularSolution: particularSolution, basisVectors: basisVectors);

    private GaussianSolution(
        SolutionType solutionType,
        double[,] matrix,
        double[] solution = null,
        double[] particularSolution = null,
        double[,] basisVectors = null)
    {
        SolutionType = solutionType;
        Matrix = matrix;
        Solution = solution;
        ParticularSolution = particularSolution;
        BasisVectors = basisVectors;
    }

    public SolutionType SolutionType { get; set; }

    public double[,] Matrix { get; set; }

    public double[] Solution { get; set; }

    public double[] ParticularSolution { get; set; }

    public double[,] BasisVectors { get; set; }

    public bool Equals(GaussianSolution other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return SolutionType == other.SolutionType
               && Matrix.ContentEquals(other.Matrix)
               && Solution.ContentEquals(other.Solution)
               && ParticularSolution.ContentEquals(other.ParticularSolution)
               && BasisVectors.ContentEquals(other.BasisVectors);
    }

    public override bool Equals(object obj)
    {
        if (obj is null)
            return false;
        if (ReferenceEquals(this, obj))
            return true;
        return obj.GetType() == GetType() && Equals((GaussianSolution)obj);
    }

    public override int GetHashCode()
    {
        throw new InvalidOperationException();
    }

    public static bool operator ==(GaussianSolution left, GaussianSolution right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(GaussianSolution left, GaussianSolution right)
    {
        return !Equals(left, right);
    }
}