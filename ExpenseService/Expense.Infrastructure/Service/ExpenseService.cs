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
    }
}
