namespace User.Domain.Models;
public class UserExpensesResponse
{
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public List<Expense> Expenses { get; set; } = new();
}
