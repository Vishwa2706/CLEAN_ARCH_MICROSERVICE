using System.Linq;
using Expense.Domain.Models;

namespace Expense.Application.Contracts
{
    public interface IExpenseService
    {
        IQueryable<ExpenseDto> GetAllExpenses();

        Task AddExpenseAsync(ExpenseDto expense);

        Task UpdateExpenseAsync(int id, ExpenseDto expense);

        Task PatchExpenseAsync(int id, PatchExpenseRequest request);
    }
}
