namespace Expense.Application.Models;

public class ExpenseSummary
{
    public string Type { get; }
    public decimal TotalAmount { get; }

    public ExpenseSummary(string type, decimal totalAmount)
    {
        Type = type;
        TotalAmount = totalAmount;
    }
}
