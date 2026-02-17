using Microsoft.EntityFrameworkCore;
using User.Domain.Models;

namespace User.Infrastructure.Repository;

public class UserRepository : DbContext
{
    public UserRepository(DbContextOptions<UserRepository> options)
        : base(options) { }

    public DbSet<UserDto> Users => Set<UserDto>();

    public DbSet<Expense> Expenses => Set<Expense>();

    public DbSet<RefTerm> RefTerms { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<UserDto>()
            .HasMany(u => u.Expenses)
            .WithOne()
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
