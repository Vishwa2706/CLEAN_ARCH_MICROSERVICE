using System.Linq;
using Expense.Domain.Models;

namespace Expense.Application.Contracts
{
    public interface IExpenseService
    {
        IQueryable<ExpenseDto> GetAllExpenses();

        Task AddExpenseAsync(ExpenseDto expense);

        Task UpdateExpenseAsync(int id, ExpenseDto expense, CancellationToken cancellationToken);

        Task PatchExpenseAsync(int id, PatchExpenseRequest request);

        Task DeleteExpenseAsync(int id);
    }
}
