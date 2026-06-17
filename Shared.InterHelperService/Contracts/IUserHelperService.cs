using Shared.InterHelperService.Protos;

namespace Shared.InterHelperService.Contracts;

public interface IUserHelperService
{
    Task<GetUserResponse?> GetUserAsync(int userId, CancellationToken cancellationToken = default);
}
