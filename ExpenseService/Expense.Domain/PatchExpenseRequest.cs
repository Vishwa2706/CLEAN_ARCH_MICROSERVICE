namespace Expense.Domain.Models
{
    public class PatchExpenseRequest
    {
        public int? UserId { get; set; }
        public string? Category { get; set; }
        public decimal? Amount { get; set; }
        public DateTime? Date { get; set; }
        public string? Note { get; set; }
        public Guid Version { get; set; }
    }
}
