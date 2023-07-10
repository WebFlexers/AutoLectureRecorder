using AutoLectureRecorder.Application.Common.Logging;
using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AutoLectureRecorder.Application.Common.Behaviors;

public class ErrorLoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : IErrorOr
{
    private readonly ILogger<TRequest> _logger;

    // ReSharper disable once ContextualLoggerProblem
    public ErrorLoggingBehavior(ILogger<TRequest> logger)
    {
        _logger = logger;
    }
    
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var response = await next();

        if (response.IsError)
        {
            foreach (var error in response.Errors!)
            {
                _logger.EfficientlyLogWarning(error.Description, error.Code);
            }
        }

        return response;
    }
}