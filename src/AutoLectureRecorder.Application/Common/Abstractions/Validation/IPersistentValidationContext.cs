namespace AutoLectureRecorder.Application.Common.Abstractions.Validation;

public interface IPersistentValidationContext
{
    /// <summary>
    /// Adds the given parameter to be used by validators of the given type
    /// </summary>
    void AddValidationParameter(Type type, string parameterIdentifier, object parameterValue);

    /// <summary>
    /// Removes all the validation parameters of the given type
    /// </summary>
    bool RemoveValidationParametersOfType(Type type);

    /// <summary>
    /// Gets the validation parameters of the given type with the specified name
    /// </summary>
    object? GetValidationParameter(Type type, string parameterIdentifier);

    /// <summary>
    /// Gets all the validation parameters of the given type
    /// </summary>
    Dictionary<string, object>? GetAllValidationParameters(Type type);
}