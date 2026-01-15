using System.Linq;
using Expense.Application.Contracts;
using Expense.Domain.Models;

namespace Expense.Application.Commands
{
    public class UpdateExpenseCommand
    {
        private readonly IExpenseService _expenseService;

        public UpdateExpenseCommand(IExpenseService expenseService)
        {
            _expenseService = expenseService;
        }

        public async Task<int> Execute(int id, CreateExpenseRequest request)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid expense id");

            if (string.IsNullOrWhiteSpace(request.Category))
                throw new ArgumentException("Category is required");

            if (request.UserId <= 0)
                throw new ArgumentException("Invalid user id");

            if (request.Amount <= 0)
                throw new ArgumentException("Amount must be greater than zero");

            var expense = new ExpenseDto
            {
                Id = id,
                UserId = request.UserId,
                Category = request.Category,
                Amount = request.Amount,
                Date = request.Date ?? DateTime.UtcNow,
                Note = request.Note,
            };

            await _expenseService.UpdateExpenseAsync(id, expense);

            return id;
        }
    }
}
