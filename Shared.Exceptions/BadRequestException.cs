using Microsoft.AspNetCore.Http;

namespace Shared.Exceptions
{
    public class BadRequestException : BaseCustomException
    {
        public BadRequestException(
            string message,
            string description,
            string errorCode = "BAD_REQUEST"
        )
            : base(message, description, StatusCodes.Status400BadRequest, errorCode) { }
    }
}
