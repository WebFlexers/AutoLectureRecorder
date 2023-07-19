using AutoLectureRecorder.Application.Common.Abstractions.Validation;

namespace AutoLectureRecorder.Infrastructure.Validation;

public class PersistentValidationContext : IPersistentValidationContext
{
    private Dictionary<string, object> _contextData = new();
    
    public void AddValidationParameter(string parameterIdentifier, object parameterValue)
    {
        _contextData[parameterIdentifier] = parameterValue;
    }
    
    public object? GetValidationParameter(string parameterIdentifier)
    {
        _contextData.TryGetValue(parameterIdentifier, out var parameter);
        return parameter;
    }
    
    public void RemoveAllValidationParameters(string type)
    {
        _contextData.Clear();
        _contextData = new();
    }
}
