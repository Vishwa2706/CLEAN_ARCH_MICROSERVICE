using Expense.Application.Contracts;
using Expense.Domain.Models;
using MediatR;
using Shared.Exceptions;

namespace Expense.Application.Commands;

public class CreateExpenseCommand : IRequest<int>
{
    public CreateExpenseRequest Request { get; set; }

    public CreateExpenseCommand(CreateExpenseRequest request)
    {
        Request = request;
    }
}

public class CreateExpenseCommandHandler : IRequestHandler<CreateExpenseCommand, int>
{
    private readonly IExpenseService _expenseService;
    private readonly IUserServiceClient _userClient;

    public CreateExpenseCommandHandler(
        IExpenseService expenseService,
        IUserServiceClient userClient
    )
    {
        _expenseService = expenseService;
        _userClient = userClient;
    }

    public async Task<int> Handle(CreateExpenseCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;

        if (string.IsNullOrWhiteSpace(request.Category))
            throw new BadRequestException(
                "Empty Category",
                "Category is required",
                "CATEGORY_INVALID"
            );

        if (request.Amount <= 0)
            throw new BadRequestException(
                "Invalid Amount",
                "Amount must be greater than zero",
                "INVALID_AMOUNT"
            );

        if (request.UserId <= 0)
            throw new BadRequestException(
                "Invalid User Id",
                "User id must be greater than zero",
                "INVALID_USER_ID"
            );

        var user = await _userClient.GetUser(request.UserId);

        if (user == null)
            throw new BadRequestException("Invalid User", "User does not exist", "USER_NOT_FOUND");

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
