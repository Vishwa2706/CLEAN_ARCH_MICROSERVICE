using IdentityService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Infrastructure.Repository;

public class IdentityRepository : DbContext
{
    public IdentityRepository(DbContextOptions<IdentityRepository> options)
        : base(options) { }

    public DbSet<User> Users => Set<User>();
}
