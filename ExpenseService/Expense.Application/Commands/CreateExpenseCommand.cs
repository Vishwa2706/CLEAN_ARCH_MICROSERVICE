using System.Linq;
using Expense.Application.Contracts;
using Expense.Domain.Models;

namespace Expense.Application.Commands
{
    public class CreateExpenseCommand
    {
        private readonly IExpenseService _expenseService;

        public CreateExpenseCommand(IExpenseService expenseService)
        {
            _expenseService = expenseService;
        }

        public async Task<int> Execute(CreateExpenseRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Category))
                throw new ArgumentException("Category is required");

            if (request.Amount <= 0)
                throw new ArgumentException("Amount must be greater than zero");

            var expense = new ExpenseDto
            {
                Category = request.Category,
                Amount = request.Amount,
                Date = request.Date ?? DateTime.UtcNow,
                Note = request.Note,
            };

            await _expenseService.AddExpenseAsync(expense);

            return expense.Id;
        }
    }
}
