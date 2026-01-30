namespace Shared.Exceptions;

public sealed class ApiErrorResponse
{
    public int Status { get; init; }
    public string Code { get; init; } = null!;
    public string Message { get; init; } = null!;
    public string Description { get; init; } = null!;
    public string TraceId { get; init; } = null!;
}
