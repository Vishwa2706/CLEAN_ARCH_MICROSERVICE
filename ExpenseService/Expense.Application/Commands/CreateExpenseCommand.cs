using Expense.Application.Contracts;
using Expense.Domain.Models;
using FluentValidation;
using MediatR;
using Shared.Common.Contracts;
using Shared.Common.Events;
using Shared.Exceptions;
using Shared.Logging.Contracts;

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
    
    private readonly ILoggerManager<CreateExpenseCommandHandler> _logger;

    public CreateExpenseCommandHandler(
        IExpenseService expenseService,
        IUserServiceClient userClient,
        IMessagePublisher publisher,
        IUnitOfWork unitOfWork,
        ILoggerManager<CreateExpenseCommandHandler> logger
    )
    {
        _expenseService = expenseService;
        _userClient = userClient;
        _publisher = publisher;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<int> Handle(CreateExpenseCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;

        _logger.LogInformation(
            "Expense creation started. UserId={UserId}, Category={Category}, Amount={Amount}",
            request.UserId,
            request.Category,
            request.Amount
        );

        var user = await _userClient.GetUser(request.UserId);

        if (user == null)
        {
            _logger.LogWarning("User validation failed. UserId={UserId}", request.UserId);

            throw new BadRequestException("Invalid User", "User does not exist", "USER_NOT_FOUND");
        }

        _logger.LogDebug("User validation succeeded. UserId={UserId}", request.UserId);

        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        _logger.LogDebug("Transaction started for UserId={UserId}", request.UserId);

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

            _logger.LogInformation(
                "Expense persisted successfully. ExpenseId={ExpenseId}",
                expense.Id
            );

            _logger.LogInformation(
                "Publishing ExpenseCreated event. ExpenseId={ExpenseId}",
                expense.Id
            );

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

            _logger.LogInformation(
                "ExpenseCreated event published successfully. ExpenseId={ExpenseId}",
                expense.Id
            );

            await _unitOfWork.CommitAsync(cancellationToken);

            _logger.LogInformation(
                "Transaction committed successfully. ExpenseId={ExpenseId}",
                expense.Id
            );

            _logger.LogInformation(
                "Expense creation completed successfully. ExpenseId={ExpenseId}",
                expense.Id
            );

            return expense.Id;
        }
        catch (Exception ex)
        {
            _logger.LogError("Expense creation failed for UserId={UserId}", ex, request.UserId);

            await _unitOfWork.RollbackAsync(cancellationToken);

            _logger.LogWarning("Transaction rolled back for UserId={UserId}", request.UserId);

            throw;
        }
    }
}
