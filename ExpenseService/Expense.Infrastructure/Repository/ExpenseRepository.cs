using Expense.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Expense.Infrastructure.Repository;

public class ExpenseRepository : DbContext
{
    public ExpenseRepository(DbContextOptions<ExpenseRepository> options)
        : base(options) { }

    public DbSet<ExpenseDto> Expenses => Set<ExpenseDto>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ExpenseDto>().Property(e => e.Version).IsConcurrencyToken();
    }
}
