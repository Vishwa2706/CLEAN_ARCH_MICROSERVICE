namespace Expense.Domain.Models
{
    public class CreateExpenseRequest
    {
        public int UserId { get; set; }
        public string Category { get; set; } = string.Empty;
        public decimal Amount { get; set; }

        public DateTime? Date { get; set; }
        public string? Note { get; set; }
    }
}
