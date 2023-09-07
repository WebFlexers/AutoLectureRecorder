namespace AutoLectureRecorder.Application.Common.Abstractions.Validation;

public interface IPersistentValidationContext
{
    void AddValidationParameter(string parameterIdentifier, object parameterValue);
    object? GetValidationParameter(string parameterIdentifier);
    void RemoveAllValidationParameters();
}