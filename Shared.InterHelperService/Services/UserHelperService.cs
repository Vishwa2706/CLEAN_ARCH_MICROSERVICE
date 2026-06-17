using Shared.InterHelperService.Contracts;
using Shared.InterHelperService.Protos;

namespace Shared.InterHelperService.Services;

public class UserHelperService : IUserHelperService
{
    private readonly UserGrpcService.UserGrpcServiceClient _client;

    public UserHelperService(UserGrpcService.UserGrpcServiceClient client)
    {
        _client = client;
    }

    public async Task<GetUserResponse?> GetUserAsync(
        int userId,
        CancellationToken cancellationToken = default
    )
    {
        var response = await _client.GetUserByIdAsync(
            new GetUserRequest { UserId = userId },
            cancellationToken: cancellationToken
        );

        return response;
    }
}
