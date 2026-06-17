using Grpc.Core;
using Shared.InterHelperService.Protos;
using Shared.Logging.Contracts;
using User.Application.Query;

namespace User.API.Grpc;

public class UserGrpcServiceImpl : UserGrpcService.UserGrpcServiceBase
{
    private readonly GetUserByUserId _getUserByUserId;

    private readonly ILoggerManager<UserGrpcServiceImpl> _logger;

    public UserGrpcServiceImpl(
        GetUserByUserId getUserByUserId,
        ILoggerManager<UserGrpcServiceImpl> logger
    )
    {
        _getUserByUserId = getUserByUserId;
        _logger = logger;
    }

    public override async Task<GetUserResponse> GetUserById(
        GetUserRequest request,
        ServerCallContext context
    )
    {
        var user = await _getUserByUserId.Execute(request.UserId);

        _logger.LogInformation("User exists");

        if (user == null)
        {
            return new GetUserResponse { Exists = false };
        }

        return new GetUserResponse
        {
            Exists = true,
            Id = user.Id,
            Name = user.Name,
            Mobile = user.Mobile,
            Role = user.Role,
        };
    }
}
