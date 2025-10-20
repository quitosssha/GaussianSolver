using Microsoft.Extensions.Logging;

namespace GaussianSolver.Extensions;

public static partial class LoggerExtensions
{
    [LoggerMessage(Level = LogLevel.Trace, Message = "{p1}{NewLine}{p2}")]
    private static partial void LogTraceParts(this ILogger logger, string p1, string newLine, string p2);
    
    public static void LogTraceParts(this ILogger logger, string step, string matrix) =>
        logger.LogTraceParts(step, Environment.NewLine, matrix);
    
    [LoggerMessage(Level = LogLevel.Debug, Message = "{p1}{NewLine}{p2}")]
    private static partial void LogDebugParts(this ILogger logger, string p1, string newLine, string p2);
    
    public static void LogDebugParts(this ILogger logger, string step, string matrix) =>
        logger.LogDebugParts(step, Environment.NewLine, matrix);
}