using User.Application.Contracts;
using User.Domain.Models;

namespace User.Application.Query;

public class GetUserByUserId
{
    private readonly IUserService _userService;

    public GetUserByUserId(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<UserDto?> Execute(int userId)
    {
        var user = await _userService.GetUserByIdAsync(userId);

        return user;
    }
}
