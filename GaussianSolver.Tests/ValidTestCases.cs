using System.Collections;
using GaussianSolver.Models;
using NUnit.Framework;

namespace GaussianSolver.Tests;

public class ValidTestCases
{
    public static IEnumerable<TestCaseData> GetAll() =>
        GetUnique()
            .Concat(GetInfinite())
            .Concat(GetNone());

    public static IEnumerable<TestCaseData> GetUnique()
    {
        yield return Natural2X2().SetName("Unique01_" + nameof(Natural2X2));
        yield return Rational3X3().SetName("Unique02_" + nameof(Rational3X3));
        yield return Echelon3X3().SetName("Unique03_" + nameof(Echelon3X3));
        yield return ReducedEchelon3X3().SetName("Unique04_" + nameof(ReducedEchelon3X3));
        yield return NeedSwap3X3().SetName("Unique05_" + nameof(NeedSwap3X3));
    }

    public static IEnumerable<TestCaseData> GetInfinite()
    {
        yield return InfiniteZero1X2().SetName("Infinite01_" + nameof(InfiniteZero1X2));
        yield return InfiniteCommon2X3().SetName("Infinite02_" + nameof(InfiniteCommon2X3));
        yield return InfiniteDegenerate3X3().SetName("Infinite03_" + nameof(InfiniteDegenerate3X3));
        yield return InfiniteDoubleDegenerate3X3().SetName("Infinite04_" + nameof(InfiniteDoubleDegenerate3X3));
        yield return InfiniteSkipStep2X4().SetName("Infinite05_" + nameof(InfiniteSkipStep2X4));
    }

    public static IEnumerable<TestCaseData> GetNone()
    {
        yield return Incompatible1X1().SetName("None01_" + nameof(Incompatible1X1));
        yield return Incompatible2X2().SetName("None02_" + nameof(Incompatible2X2));
        yield return Incompatible4X3().SetName("None03_" + nameof(Incompatible4X3));
    }

    #region Unique Test Cases

    private static TestCaseData Natural2X2()
    {
        var matrix = new double[,]
        {
            { 2, 1, 5 },
            { 1, -1, 1 }
        };
        var result = GaussianSolution.Unique(null, [2, 1]);
        return new TestCaseData(matrix, result);
    }

    private static TestCaseData Rational3X3()
    {
        var matrix = new double[,]
        {
            { 1, 2, 1, 8 },
            { 2, 1, -1, 1 },
            { 1, -1, 2, 3 }
        };
        var result = GaussianSolution.Unique(null, [0.5, 2.5, 2.5]);
        return new TestCaseData(matrix, result);
    }

    private static TestCaseData Echelon3X3()
    {
        var matrix = new double[,]
        {
            { 2, 3, -1, 7 },
            { 0, 4, 2, 10 },
            { 0, 0, 3, 9 }
        };
        var result = GaussianSolution.Unique(null, [3.5, 1, 3]);
        return new TestCaseData(matrix, result);
    }

    private static TestCaseData ReducedEchelon3X3()
    {
        var matrix = new double[,]
        {
            { 1, 0, 0, 4 },
            { 0, 1, 0, 5 },
            { 0, 0, 1, 6 }
        };
        var result = GaussianSolution.Unique(null, [4, 5, 6]);
        return new TestCaseData(matrix, result);
    }

    private static TestCaseData NeedSwap3X3()
    {
        var matrix = new double[,]
        {
            { 0, 1, 3, 4 },
            { 0, 0, 1, 3 },
            { 1, 0, 1, 6 }
        };
        var result = GaussianSolution.Unique(null, [3, -5, 3]);
        return new TestCaseData(matrix, result);
    }

    #endregion

    #region Infinite Test Cases

    private static TestCaseData InfiniteZero1X2()
    {
        var matrix = new double[,]
        {
            { 0, 0, 0 }
        };
        var basisVectors = new double[,]
        {
            { 1, 0 },
            { 0, 1 }
        };
        var result = GaussianSolution.Infinite(null, [0, 0], basisVectors);
        return new TestCaseData(matrix, result);
    }

    private static TestCaseData InfiniteCommon2X3()
    {
        var matrix = new double[,]
        {
            { 1, 1, 1, 6 },
            { 2, -1, 1, 3 }
        };
        var basisVectors = new double[,]
        {
            { -0.6666666666666667, -0.3333333333333333, 1.0 }
        };
        var result = GaussianSolution.Infinite(null, [3, 3, 0], basisVectors);
        return new TestCaseData(matrix, result);
    }

    private static TestCaseData InfiniteDegenerate3X3()
    {
        var matrix = new double[,]
        {
            { 1, 1, 1, 6 },
            { 2, 4, 6, 12 },
            { 3, 6, 9, 18 }
        };
        var basisVectors = new double[,]
        {
            { 1.0, -2.0, 1.0 }
        };
        var result = GaussianSolution.Infinite(null, [6, 0, 0], basisVectors);
        return new TestCaseData(matrix, result);
    }

    private static TestCaseData InfiniteDoubleDegenerate3X3()
    {
        var matrix = new double[,]
        {
            { 1, 1, 1, 3 },
            { 2, 2, 2, 6 },
            { 3, 3, 3, 9 }
        };
        var basisVectors = new double[,]
        {
            { -1.0, 1.0, 0.0 },
            { -1.0, 0.0, 1.0 }
        };
        var result = GaussianSolution.Infinite(null, [3, 0, 0], basisVectors);
        return new TestCaseData(matrix, result);
    }

    private static TestCaseData InfiniteSkipStep2X4()
    {
        var matrix = new double[,]
        {
            { 2, 0, 3, 5, 3 },
            { 1, 0, -1, 5, 9 }
        };
        var basisVectors = new double[,]
        {
            { 0, 1, 0, 0 },
            { -4, 0, 1, 1 }
        };
        var result = GaussianSolution.Infinite(null, [6, 0, -3, 0], basisVectors);
        return new TestCaseData(matrix, result);
    }

    #endregion

    #region None Test Cases

    private static TestCaseData Incompatible1X1()
    {
        var matrix = new double[,]
        {
            { 0, 1 }
        };
        var result = GaussianSolution.None(null);
        return new TestCaseData(matrix, result);
    }

    private static TestCaseData Incompatible2X2()
    {
        var matrix = new double[,]
        {
            { 1, 1, 5 },
            { 1, 1, 3 }
        };
        var result = GaussianSolution.None(null);
        return new TestCaseData(matrix, result);
    }

    private static TestCaseData Incompatible4X3()
    {
        var matrix = new double[,]
        {
            { 1, 1, 1, 6 },
            { 2, -1, 1, 3 },
            { 3, 0, 2, 5 },
            { 1, 2, 0, 1 }
        };
        var result = GaussianSolution.None(null);
        return new TestCaseData(matrix, result);
    }

    #endregion
}