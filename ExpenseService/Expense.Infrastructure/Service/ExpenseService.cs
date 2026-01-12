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

        public async Task UpdateExpenseAsync(int id, ExpenseDto expense)
        {
            var existingExpense = await _context.Expenses.FindAsync(id);

            if (existingExpense == null)
                throw new ArgumentException("Expense not found");

            existingExpense.Category = expense.Category;
            existingExpense.Amount = expense.Amount;
            existingExpense.Date = expense.Date;
            existingExpense.Note = expense.Note;

            await _context.SaveChangesAsync();
        }
    }
}
