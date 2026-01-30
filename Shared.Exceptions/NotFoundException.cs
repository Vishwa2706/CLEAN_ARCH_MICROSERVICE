using Microsoft.AspNetCore.Http;

namespace Shared.Exceptions
{
    public class NotFoundException : BaseCustomException
    {
        public NotFoundException(string message, string description, string errorCode = "NOT_FOUND")
            : base(message, description, StatusCodes.Status404NotFound, errorCode) { }
    }
}
