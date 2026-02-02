using IdentityService.Application.Contracts;
using IdentityService.Domain.Entities;
using IdentityService.Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Infrastructure.Service;

public class UserService : IUserService
{
    private readonly IdentityRepository _db;

    public UserService(IdentityRepository db)
    {
        _db = db;
    }

    public async Task<User?> GetByMobileAsync(long mobile)
    {
        return await _db.Users.FirstOrDefaultAsync(x => x.Mobile == mobile);
    }

    public async Task AddAsync(User user)
    {
        _db.Users.Add(user);
        await _db.SaveChangesAsync();
    }
}
