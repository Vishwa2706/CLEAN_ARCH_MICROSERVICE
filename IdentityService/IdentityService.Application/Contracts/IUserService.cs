using System.Linq;
using IdentityService.Domain.Entities;

namespace IdentityService.Application.Contracts
{
    public interface IUserService
    {
        Task<User?> GetByMobileAsync(long mobile);
        Task AddAsync(User user);
    }
}
