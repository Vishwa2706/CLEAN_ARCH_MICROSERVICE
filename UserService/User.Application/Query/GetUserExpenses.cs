using System.Linq;
using User.Application.Contracts;
using User.Domain.Models;

namespace User.Application.Query;

public class GetUserExpensesQuery
{
    private readonly IUserService _userService;

    public GetUserExpensesQuery(IUserService userService)
    {
        _userService = userService;
    }

    public Task<UserExpensesResponse?> ExecuteAsync(int userId)
    {
        return _userService.GetUserExpensesAsync(userId);
    }
}

