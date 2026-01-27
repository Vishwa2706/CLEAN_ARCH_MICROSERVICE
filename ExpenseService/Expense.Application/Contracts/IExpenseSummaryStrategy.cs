using Expense.Application.Models;
using Expense.Domain.Models;

namespace Expense.Application.Contracts;

public interface IExpenseSummaryStrategy
{
    ExpenseSummary Calculate(IQueryable<ExpenseDto> expenses, string? month, string? category);
}
