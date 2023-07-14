using AutoLectureRecorder.Application.Common.Abstractions.Validation;
using ErrorOr;
using FluentValidation;
using MediatR;

namespace AutoLectureRecorder.Application.Common.Behaviors;

public class ValidationBehavior<TRequest, TResponse> :
    IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : IErrorOr
{
    private readonly IPersistentValidationContext _persistentValidationContext;
    private readonly IValidator<TRequest>? _validator;

    public ValidationBehavior(IPersistentValidationContext persistentValidationContext, 
        IValidator<TRequest>? validator = null)
    {
        _persistentValidationContext = persistentValidationContext;
        _validator = validator;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (_validator is null)
        {
            return await next();
        }

        var requestType = typeof(TRequest);

        var fluentValidationContext = new ValidationContext<TRequest>(request);
        var validationParameters = _persistentValidationContext
            .GetAllValidationParameters(requestType);

        if (validationParameters is not null)
        {
            foreach (var parameterKeyValuePair in validationParameters)
            {
                fluentValidationContext.RootContextData[parameterKeyValuePair.Key] = parameterKeyValuePair.Value;
            }
        }

        var validationResult = await _validator.ValidateAsync(fluentValidationContext, cancellationToken);

        _persistentValidationContext.RemoveValidationParametersOfType(requestType);
        
        if (validationResult.IsValid)
        {
            return await next();
        }

        

        var errors = validationResult.Errors
            .ConvertAll(validationFailure =>
            {
                string errorCode;
                if (validationFailure.ErrorCode == "OverlappingLecture" || 
                    string.IsNullOrWhiteSpace(validationFailure.PropertyName))
                {
                    errorCode = validationFailure.ErrorCode;
                }
                else
                {
                    errorCode = validationFailure.PropertyName;
                }
                return Error.Validation(
                    errorCode,
                    validationFailure.ErrorMessage);
            });

        return (dynamic)errors;
    }
}