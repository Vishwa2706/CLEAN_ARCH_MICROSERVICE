using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.AspNetCore.Http;

namespace Shared.Common.Interceptors;

public class CorrelationIdGrpcInterceptor : Interceptor
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CorrelationIdGrpcInterceptor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(
        TRequest request,
        ClientInterceptorContext<TRequest, TResponse> context,
        AsyncUnaryCallContinuation<TRequest, TResponse> continuation
    )
        where TRequest : class
        where TResponse : class
    {
        var correlationId = _httpContextAccessor.HttpContext?.Items["CorrelationId"]?.ToString();

        var headers = context.Options.Headers ?? new Metadata();

        if (!string.IsNullOrWhiteSpace(correlationId))
        {
            headers.Add("X-Correlation-Id", correlationId);
        }

        var options = context.Options.WithHeaders(headers);

        var newContext = new ClientInterceptorContext<TRequest, TResponse>(
            context.Method,
            context.Host,
            options
        );

        return continuation(request, newContext);
    }
}
