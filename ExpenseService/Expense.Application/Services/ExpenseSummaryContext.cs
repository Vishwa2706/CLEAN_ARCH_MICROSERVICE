using Expense.Application.Contracts;
using Expense.Application.Models;
using Expense.Application.Strategies;
using Expense.Domain.Models;

namespace Expense.Application.Services;

public class ExpenseSummaryContext
{
    private IExpenseSummaryStrategy _strategy;

    public ExpenseSummaryContext(IExpenseSummaryStrategy strategy)
    {
        _strategy = strategy;
    }

    public ExpenseSummary Generate(IQueryable<ExpenseDto> expenses, string? month, string? category)
    {
        return _strategy.Calculate(expenses, month, category);
    }
}
