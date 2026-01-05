using Expense.Domain.Models;

namespace Expense.Application.Contracts
{
    public interface IExpenseExporter
    {
        byte[] Export(IEnumerable<ExpenseDto> expenses);
        string ContentType { get; }
        string FileExtension { get; }
    }
}
