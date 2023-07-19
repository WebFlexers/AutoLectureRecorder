using AutoLectureRecorder.Application.Common.Abstractions.Validation;
using ErrorOr;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AutoLectureRecorder.Application.Common.Behaviors;

public class ValidationBehavior<TRequest, TResponse> :
    IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : IErrorOr
{
    private readonly IPersistentValidationContext _persistentValidationContext;
    private readonly ILogger<TRequest> _logger;
    private readonly IValidator<TRequest>? _validator;

    // ReSharper disable once ContextualLoggerProblem
    public ValidationBehavior(IPersistentValidationContext persistentValidationContext, ILogger<TRequest> logger,
        IValidator<TRequest>? validator = null)
    {
        _persistentValidationContext = persistentValidationContext;
        _logger = logger;
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

        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

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

        foreach (var error in errors)
        {
            _logger.LogWarning("Validation failed with code {Code}: {Description}",
                error.Code, error.Description);
        }

        return (dynamic)errors;
    }
}