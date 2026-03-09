using System.Linq;
using Microsoft.EntityFrameworkCore;
using User.Application.Contracts;
using User.Domain.Models;
using User.Infrastructure.Repository;

namespace User.Infrastructure.Service
{
    public class UserService : IUserService
    {
        private readonly UserRepository _context;

        public UserService(UserRepository context)
        {
            _context = context;
        }

        public IQueryable<UserDto> GetAllUsers() => _context.Users.AsNoTracking();

        public async Task<UserExpensesResponse?> GetUserExpensesAsync(int userId)
        {
            var data = await (
                from u in _context.Users
                join e in _context.Expenses on u.Id equals e.UserId into expenseGroup
                where u.Id == userId
                select new UserExpensesResponse
                {
                    UserId = u.Id,
                    UserName = u.Name,
                    Expenses = expenseGroup
                        .Select(x => new Expense
                        {
                            Id = x.Id,
                            UserId = x.UserId,
                            Category = x.Category,
                            Amount = x.Amount,
                            Date = x.Date,
                            Note = x.Note,
                        })
                        .ToList(),
                }
            )
                .AsNoTracking()
                .FirstOrDefaultAsync();

            return data;
        }

        public async Task<UserDto?> GetUserPermissions(int userId)
        {
            return await _context.Users.FirstOrDefaultAsync(u =>
                u.Id == userId && u.Role == "Admin"
            );
        }

        public async Task<UserDto?> GetUserByIdAsync(int userId)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        }
    }
}
