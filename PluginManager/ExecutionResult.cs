namespace ModularPluginAPI;

public class ExecutionResult
{
    private readonly bool _isSuccess;
    private readonly string? _errorMessage;

    private ExecutionResult(bool isSuccess, string? errorMessage)
    {
        _isSuccess = isSuccess;
        _errorMessage = errorMessage;
    }
    
    public bool IsSuccess => _isSuccess;
    public bool IsFailure => !_isSuccess;
    
    public static ExecutionResult Success() => 
        new ExecutionResult(true, null);
    public static ExecutionResult Failure(string errorMessage) => 
        new ExecutionResult(false, errorMessage);

    public static ExecutionResult FromResults(List<ExecutionResult> results)
    {
        if (results.All(r => r.IsSuccess))
            return Success();
        
        return Failure("One or more results are failed.");
    }
}