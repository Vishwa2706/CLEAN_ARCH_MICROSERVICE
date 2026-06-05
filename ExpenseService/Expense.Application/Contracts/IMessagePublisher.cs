using Shared.Common.Events;

namespace Expense.Application.Contracts;

public interface IMessagePublisher
{
    Task PublishExpenseCreatedAsync(
        ExpenseCreatedEvent message,
        CancellationToken cancellationToken = default
    );
}
