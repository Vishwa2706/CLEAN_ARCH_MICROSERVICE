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
    }
}
