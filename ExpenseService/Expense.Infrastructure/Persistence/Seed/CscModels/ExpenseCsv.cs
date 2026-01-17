namespace Expense.Infrastructure.Persistence.Seed.CsvModels;

public class ExpenseCsv
{
    public int UserId { get; set; }
    public string Category { get; set; } = null!;
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public string? Note { get; set; }
}
