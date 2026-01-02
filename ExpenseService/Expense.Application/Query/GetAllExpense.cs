using System.Linq;
using Expense.Application.Contracts;
using Expense.Domain.Models;

namespace Expense.Application.Query
{
    public class GetAllExpenseQuery
    {
        private readonly IExpenseService _expenseService;

        public GetAllExpenseQuery(IExpenseService expenseService)
        {
            _expenseService = expenseService;
        }

        public IQueryable<ExpenseDto> Execute(int startIndex, int pageSize, string? searchTerm = "")
        {
            var query = _expenseService.GetAllExpenses();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(e => e.Note.Contains(searchTerm));
            }

            query = query.OrderBy(e => e.Id).Skip(startIndex * pageSize).Take(pageSize);

            return query;
        }
    }
}
