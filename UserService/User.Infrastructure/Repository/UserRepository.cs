using User.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace User.Infrastructure.Repository;

public class UserRepository : DbContext
{

    public UserRepository(DbContextOptions<UserRepository> options)
        : base(options) { }

    public DbSet<UserDto> Users => Set<UserDto>();
}
