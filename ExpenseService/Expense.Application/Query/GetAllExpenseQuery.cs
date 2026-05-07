using Expense.Application.Contracts;
using Expense.Domain.Models;
using MediatR;

namespace Expense.Application.Query;

public class GetAllExpenseQuery : IRequest<List<ExpenseDto>>
{
    public string? SearchTerm { get; set; } = "";
    public int StartIndex { get; set; }
    public int PageSize { get; set; }
}

public class GetAllExpenseQueryHandler
    : IRequestHandler<GetAllExpenseQuery, List<ExpenseDto>>
{
    private readonly IExpenseService _expenseService;

    public GetAllExpenseQueryHandler(IExpenseService expenseService)
    {
        _expenseService = expenseService;
    }

    public Task<List<ExpenseDto>> Handle(
        GetAllExpenseQuery request,
        CancellationToken cancellationToken
    )
    {
        var query = _expenseService.GetAllExpenses();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            query = query.Where(e =>
                e.Note.Contains(request.SearchTerm)
            );
        }

        query = query
            .OrderBy(e => e.Id)
            .Skip(request.StartIndex * request.PageSize)
            .Take(request.PageSize);

        return Task.FromResult(query.ToList());
    }
}