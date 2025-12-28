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

        public IQueryable<ExpenseDto> Execute()
        {
            return _expenseService.GetAllExpenses();
        }
    }
}
