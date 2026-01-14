using System.Linq;
using Expense.Application.Contracts;

namespace Expense.Application.Commands
{
    public class DeleteExpenseCommand
    {
        private readonly IExpenseService _expenseService;

        public DeleteExpenseCommand(IExpenseService expenseService)
        {
            _expenseService = expenseService;
        }

        public async Task<int> Execute(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid Expense id");

            await _expenseService.DeleteExpenseAsync(id);

            return id;
        }
    }
}
