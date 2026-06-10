using Expense.Application.Contracts;
using Expense.Domain.Models;
using FluentValidation;
using MediatR;
using Shared.Common.Contracts;
using Shared.Common.Events;
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

// Validator

public class CreateExpenseCommandValidator : AbstractValidator<CreateExpenseCommand>
{
    public CreateExpenseCommandValidator()
    {
        RuleFor(x => x.Request.Category).NotEmpty().WithMessage("Category is required");

        RuleFor(x => x.Request.Amount)
            .GreaterThan(0)
            .WithMessage("Amount must be greater than zero");

        RuleFor(x => x.Request.UserId)
            .GreaterThan(0)
            .WithMessage("User id must be greater than zero");
    }
}

// Handler

public class CreateExpenseCommandHandler : IRequestHandler<CreateExpenseCommand, int>
{
    private readonly IExpenseService _expenseService;

    private readonly IUserServiceClient _userClient;

    private readonly IMessagePublisher _publisher;

    private readonly IUnitOfWork _unitOfWork;

    public CreateExpenseCommandHandler(
        IExpenseService expenseService,
        IUserServiceClient userClient,
        IMessagePublisher publisher,
        IUnitOfWork unitOfWork
    )
    {
        _expenseService = expenseService;
        _userClient = userClient;
        _publisher = publisher;
        _unitOfWork = unitOfWork;
    }

    public async Task<int> Handle(CreateExpenseCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;

        var user = await _userClient.GetUser(request.UserId);

        if (user == null)
        {
            throw new BadRequestException("Invalid User", "User does not exist", "USER_NOT_FOUND");
        }

        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var expense = new ExpenseDto
            {
                UserId = request.UserId,
                Category = request.Category,
                Amount = request.Amount,
                Date = request.Date ?? DateTime.UtcNow,
                Note = request.Note,
            };

            await _expenseService.AddExpenseAsync(expense);

            await _publisher.PublishExpenseCreatedAsync(
                new ExpenseCreatedEvent
                {
                    ExpenseId = expense.Id,
                    UserId = expense.UserId ?? 1,
                    Category = expense.Category,
                    Amount = expense.Amount,
                    CreatedAt = DateTime.UtcNow,
                },
                cancellationToken
            );

            await _unitOfWork.CommitAsync(cancellationToken);

            return expense.Id;
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackAsync(cancellationToken);

            throw;
        }
    }
}
