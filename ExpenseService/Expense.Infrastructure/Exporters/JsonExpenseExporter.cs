using System.Text.Json;
using Expense.Application.Contracts;
using Expense.Domain.Models;

namespace Expense.Infrastructure.Exporters
{
    public class JsonExpenseExporter : IExpenseExporter
    {
        public string ContentType => "application/json";
        public string FileExtension => "json";

        public byte[] Export(IEnumerable<ExpenseDto> expenses)
        {
            var json = JsonSerializer.Serialize(
                expenses,
                new JsonSerializerOptions { WriteIndented = true }
            );

            return System.Text.Encoding.UTF8.GetBytes(json);
        }
    }
}
