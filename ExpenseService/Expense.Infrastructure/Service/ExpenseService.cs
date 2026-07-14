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

        public async Task UpdateExpenseAsync(
            int id,
            ExpenseDto expense,
            CancellationToken cancellationToken
        )
        {
            var existingExpense = await _context.Expenses.FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken
            );

            if (existingExpense == null)
                throw new ArgumentException("Expense not found");

            existingExpense.UserId = expense.UserId;
            existingExpense.Category = expense.Category;
            existingExpense.Amount = expense.Amount;
            existingExpense.Date = expense.Date;
            existingExpense.Note = expense.Note;

            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task PatchExpenseAsync(int id, PatchExpenseRequest request)
        {
            var existingExpense = await _context.Expenses.FindAsync(id);

            if (existingExpense == null)
                throw new ArgumentException("Expense not found");

            _context.Entry(existingExpense).Property(x => x.Version).OriginalValue =
                request.Version;

            if (request.UserId.HasValue)
                existingExpense.UserId = request.UserId;

            if (!string.IsNullOrWhiteSpace(request.Category))
                existingExpense.Category = request.Category;

            if (request.Amount.HasValue)
                existingExpense.Amount = request.Amount.Value;

            if (request.Date.HasValue)
                existingExpense.Date = request.Date.Value;

            if (request.Note != null)
                existingExpense.Note = request.Note;

            // Generate a new Version for the updated record
            existingExpense.Version = Guid.NewGuid();

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "This expense has already been modified by another user."
                );
            }
        }

        public async Task DeleteExpenseAsync(int id)
        {
            var expense = await _context.Expenses.FindAsync(id);
            if (expense == null)
                throw new ArgumentException("Expense not found");

            _context.Expenses.Remove(expense);

            await _context.SaveChangesAsync();
        }
    }
}
