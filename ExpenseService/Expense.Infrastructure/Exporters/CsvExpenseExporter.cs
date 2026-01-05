using System.Text;
using Expense.Application.Contracts;
using Expense.Domain.Models;

namespace Expense.Infrastructure.Exporters
{
    public class CsvExpenseExporter : IExpenseExporter
    {
        public string ContentType => "text/csv";
        public string FileExtension => "csv";

        public byte[] Export(IEnumerable<ExpenseDto> expenses)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Id,Category,Amount,Date,Note");

            foreach (var e in expenses)
            {
                sb.AppendLine($"{e.Id},{e.Category},{e.Amount},{e.Date:yyyy-MM-dd},{e.Note}");
            }

            return Encoding.UTF8.GetBytes(sb.ToString());
        }
    }
}
