namespace Shared.Exceptions;

[Serializable]
public class BaseCustomException : Exception
{
    public int StatusCode { get; }
    public string ErrorCode { get; }
    public string Description { get; }

    protected BaseCustomException(
        string message,
        string description,
        int statusCode,
        string errorCode
    )
        : base(message)
    {
        StatusCode = statusCode;
        ErrorCode = errorCode;
        Description = description;
    }
}
