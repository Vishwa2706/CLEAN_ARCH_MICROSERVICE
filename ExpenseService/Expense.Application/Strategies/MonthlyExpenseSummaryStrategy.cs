using Expense.Application.Contracts;
using Expense.Application.Models;
using Expense.Application.Strategies;
using Expense.Domain.Models;

namespace Expense.Application.Strategies;

public class MonthlyExpenseSummaryStrategy : IExpenseSummaryStrategy
{
    public ExpenseSummary Calculate(
        IQueryable<ExpenseDto> expenses,
        string? month,
        string? category
    )
    {
        if (string.IsNullOrWhiteSpace(month))
            throw new ArgumentException("Month is required for monthly summary");

        var monthNumber = ParseMonth(month);

        var total = expenses
            .Where(e => e.Date.Month == monthNumber)
            .Sum(e => e.Amount);

        return new ExpenseSummary(month, total);
    }

    private static int ParseMonth(string month)
    {
        month = month.Trim().ToLower();

        return month switch
        {
            "jan" or "january" => 1,
            "feb" or "february" => 2,
            "mar" or "march" => 3,
            "apr" or "april" => 4,
            "may" => 5,
            "jun" or "june" => 6,
            "jul" or "july" => 7,
            "aug" or "august" => 8,
            "sep" or "september" => 9,
            "oct" or "october" => 10,
            "nov" or "november" => 11,
            "dec" or "december" => 12,
            _ => throw new ArgumentException("Invalid month value")
        };
    }
}

