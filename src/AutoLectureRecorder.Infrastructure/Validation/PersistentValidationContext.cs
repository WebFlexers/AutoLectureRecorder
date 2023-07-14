using AutoLectureRecorder.Application.Common.Abstractions.Validation;

namespace AutoLectureRecorder.Infrastructure.Validation;

public class PersistentValidationContext : IPersistentValidationContext
{
    private readonly Dictionary<Type, Dictionary<string, object>> _contextData = new();

    /// <inheritdoc/>
    public void AddValidationParameter(Type type, string parameterIdentifier, object parameterValue)
    {
        if (_contextData.ContainsKey(type) == false)
        {
            _contextData[type] = new Dictionary<string, object>();
        }

        _contextData[type][parameterIdentifier] = parameterValue;
    }

    /// <inheritdoc/>
    public bool RemoveValidationParametersOfType(Type type)
    {
        return _contextData.Remove(type);
    }

    /// <inheritdoc/>
    public object? GetValidationParameter(Type type, string parameterIdentifier)
    {
        if (_contextData.TryGetValue(type, out var parameters) == false)
        {
            return null;
        }
        
        parameters.TryGetValue(parameterIdentifier, out var parameter);
        return parameter;
    }

    /// <inheritdoc/>
    public Dictionary<string, object>? GetAllValidationParameters(Type type)
    {
        _contextData.TryGetValue(type, out var parameters);
        return parameters;
    }
}
