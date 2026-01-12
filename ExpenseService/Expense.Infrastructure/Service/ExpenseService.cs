using System.Linq;
using Expense.Application.Contracts;
using Expense.Domain.Models;
using Expense.Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;

namespace Expense.Infrastructure.Service
{
    public class ExpenseService : IExpenseService
    {
        private readonly ExpenseRepository _context;

        public ExpenseService(ExpenseRepository context)
        {
            _context = context;
        }

        public IQueryable<ExpenseDto> GetAllExpenses() => _context.Expenses.AsNoTracking();

        public async Task AddExpenseAsync(ExpenseDto expense)
        {
            _context.Expenses.Add(expense);
            await _context.SaveChangesAsync();
        }
    }
}
