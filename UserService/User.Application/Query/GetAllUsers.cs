using System.Linq;
using User.Application.Contracts;
using User.Domain.Models;

namespace User.Application.Query;

public class GetAllUserQuery
{
    private readonly IUserService _userService;

    public GetAllUserQuery(IUserService userService)
    {
        _userService = userService;
    }

    public IQueryable<UserDto> Execute()
    {
        var query = _userService.GetAllUsers();

        return query;
    }
}
