using System.Linq;
using Expense.Application.Contracts;
using Expense.Domain.Models;
using Shared.Exceptions;

namespace Expense.Application.Commands
{
    public class CreateExpenseCommand
    {
        private readonly IExpenseService _expenseService;

        private readonly IUserServiceClient _userClient;

        public CreateExpenseCommand(IExpenseService expenseService, IUserServiceClient userClient)
        {
            _expenseService = expenseService;
            _userClient = userClient;
        }

        public async Task<int> Execute(CreateExpenseRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Category))
                throw new BadRequestException(
                    "Emtpy Category",
                    "Category is required",
                    "Category_INVALID"
                );

            if (request.Amount <= 0)
                throw new BadRequestException(
                    "Invalid Amount",
                    "Amount must be greater than zero",
                    "INVALID_AMOUNT"
                );

            if (request.UserId <= 0)
                throw new BadRequestException(
                    "Invalid user id",
                    "user id must be greater than zero",
                    "INVALID_USER_ID"
                );

            var user = await _userClient.GetUser(request.UserId);

            if (user == null)
                throw new BadRequestException(
                    "Invalid User",
                    "User does not exist",
                    "USER_NOT_FOUND"
                );

            var expense = new ExpenseDto
            {
                UserId = request.UserId,
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
