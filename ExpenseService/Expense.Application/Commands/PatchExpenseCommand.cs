using System.Linq;
using Expense.Application.Contracts;
using Expense.Domain.Models;

namespace Expense.Application.Commands
{
    public class PatchExpenseCommand
    {
        private readonly IExpenseService _expenseService;

        public PatchExpenseCommand(IExpenseService expenseService)
        {
            _expenseService = expenseService;
        }

        public async Task<int> Execute(int id, PatchExpenseRequest request)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid expense id");

            if (request.Version == Guid.Empty)
                throw new ArgumentException("Version is required.");

            if (string.IsNullOrWhiteSpace(request.Category))
                throw new ArgumentException("Category is required");

            if (request.UserId <= 0)
                throw new ArgumentException("Invalid user id");

            if (request.Amount <= 0)
                throw new ArgumentException("Amount must be greater than zero");

            await _expenseService.PatchExpenseAsync(id, request);

            return id;
        }
    }
}
