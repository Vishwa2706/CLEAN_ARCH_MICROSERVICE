using Expense.Infrastructure.Repository;
using Microsoft.EntityFrameworkCore.Storage;
using Shared.Common.Contracts;

namespace Expense.Infrastructure.Persistence;

public class ExpenseUnitOfWork : IUnitOfWork
{
    private readonly ExpenseRepository _context;

    private IDbContextTransaction? _transaction;

    public ExpenseUnitOfWork(ExpenseRepository context)
    {
        _context = context;
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken)
    {
        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitAsync(CancellationToken cancellationToken)
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync(cancellationToken);
        }
    }

    public async Task RollbackAsync(CancellationToken cancellationToken)
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync(cancellationToken);
        }
    }
}
