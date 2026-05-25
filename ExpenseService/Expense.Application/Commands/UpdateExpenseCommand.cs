using Expense.Application.Contracts;
using Expense.Domain.Models;
using MediatR;

namespace Expense.Application.Commands;

public class UpdateExpenseCommand : IRequest<int>
{
    public int Id { get; set; }
    public CreateExpenseRequest Request { get; set; }

    public UpdateExpenseCommand(int id, CreateExpenseRequest request)
    {
        Id = id;
        Request = request;
    }
}

public class UpdateExpenseCommandHandler : IRequestHandler<UpdateExpenseCommand, int>
{
    private readonly IExpenseService _expenseService;

    public UpdateExpenseCommandHandler(IExpenseService expenseService)
    {
        _expenseService = expenseService;
    }

    public async Task<int> Handle(UpdateExpenseCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;

        if (command.Id <= 0)
            throw new ArgumentException("Invalid expense id");

        if (string.IsNullOrWhiteSpace(request.Category))
            throw new ArgumentException("Category is required");

        if (request.Amount <= 0)
            throw new ArgumentException("Amount must be greater than zero");

        var expense = new ExpenseDto
        {
            Id = command.Id,
            UserId = request.UserId,
            Category = request.Category,
            Amount = request.Amount,
            Date = request.Date ?? DateTime.UtcNow,
            Note = request.Note,
        };

        await _expenseService.UpdateExpenseAsync(command.Id, expense, cancellationToken);

        return command.Id;
    }
}
