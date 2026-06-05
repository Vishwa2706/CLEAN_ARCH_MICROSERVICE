namespace Shared.Common.Events;

public class ExpenseCreatedEvent
{
    public int ExpenseId { get; set; }

    public int UserId { get; set; }

    public string Category { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public DateTime CreatedAt { get; set; }
}
