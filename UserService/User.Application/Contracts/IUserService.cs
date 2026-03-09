using System.Linq;
using User.Domain.Models;

namespace User.Application.Contracts
{
    public interface IUserService
    {
        IQueryable<UserDto> GetAllUsers();

        Task<UserExpensesResponse?> GetUserExpensesAsync(int userId);

        Task<UserDto?> GetUserPermissions(int userId);

        Task<UserDto?> GetUserByIdAsync(int userId);
    }
}
