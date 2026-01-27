using Expense.Application.Contracts;
using Expense.Application.Models;
using Expense.Application.Strategies;
using Expense.Domain.Models;

namespace Expense.Application.Strategies;

public class CategoryExpenseSummaryStrategy : IExpenseSummaryStrategy
{
    public ExpenseSummary Calculate(
        IQueryable<ExpenseDto> expenses,
        string? month,
        string? category
    )
    {
        if (string.IsNullOrWhiteSpace(category))
            throw new ArgumentException("Category is required for category summary");

        var total = expenses
            .Where(e => e.Category.ToLower() == category.ToLower())
            .Sum(e => e.Amount);

        return new ExpenseSummary(category, total);
    }
}
