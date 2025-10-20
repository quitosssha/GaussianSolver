namespace GaussianSolver.Validation;

public class ValidationResult
{
    public bool IsValid { get; }
    public string Message { get; }

    private ValidationResult(bool isValid, string message)
    {
        IsValid = isValid;
        Message = message;
    }

    public static ValidationResult Valid(string message) => 
        new(true, message);
    
    public static ValidationResult Invalid(string message) => 
        new(false, message);
}