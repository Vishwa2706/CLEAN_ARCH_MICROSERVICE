using Expense.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Expense.Infrastructure.Repository;

public class ExpenseRepository : DbContext
{
    public ExpenseRepository(DbContextOptions<ExpenseRepository> options)
        : base(options) { }

    public DbSet<ExpenseDto> Expenses => Set<ExpenseDto>();
}
