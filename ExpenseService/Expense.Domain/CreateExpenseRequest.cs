namespace Expense.Domain.Models
{
    public class CreateExpenseRequest
    {
        public string Category { get; set; } = string.Empty;
        public decimal Amount { get; set; }

        public DateTime? Date { get; set; }
        public string? Note { get; set; }
    }
}
