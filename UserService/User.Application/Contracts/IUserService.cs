using System.Linq;
using User.Domain.Models;

namespace User.Application.Contracts
{
    public interface IUserService
    {
        IQueryable<UserDto> GetAllUsers();

        Task<UserExpensesResponse?> GetUserExpensesAsync(int userId);
    }
}
