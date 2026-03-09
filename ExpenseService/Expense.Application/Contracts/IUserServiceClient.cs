using Expense.Domain.Models;

namespace Expense.Application.Contracts;

public interface IUserServiceClient
{
    Task<UserDto?> GetUser(int id);
}
