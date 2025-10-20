namespace GaussianSolver.Models;

public class GaussianSolution
{
    public static GaussianSolution None(double[,] matrix) => 
        new(SolutionType.None, matrix);
    
    public static GaussianSolution Unique(double[,] matrix, double[] solution) => 
        new(SolutionType.Unique, matrix, solution);
    
    public static GaussianSolution Infinite(double[,] matrix, double[] particularSolution, double[,] basisVectors) => 
        new(SolutionType.Infinite, matrix, particularSolution: particularSolution, basicVectors: basisVectors);
    
    private GaussianSolution(
        SolutionType solutionType,
        double[,] matrix,
        double[] solution = null,
        double[] particularSolution = null,
        double[,] basicVectors = null)
    {
        SolutionType = solutionType;
        Matrix = matrix;
        Solution = solution;
        ParticularSolution = particularSolution;
        BasicVectors = basicVectors;
    }

    public SolutionType SolutionType { get; }
    
    public double[,] Matrix { get; }

    public double[] Solution { get; }

    public double[] ParticularSolution { get; }

    public double[,] BasicVectors { get; }
}

public enum SolutionType
{
    None,
    Unique,
    Infinite
}