using Expense.Application.Contracts;
using Expense.Domain.Models;
using Parquet.Serialization;

namespace Expense.Infrastructure.Exporters
{
    public class ParquetExpenseExporter : IExpenseExporter
    {
        public string ContentType => "application/vnd.apache.parquet";

        public string FileExtension => "parquet";

        public byte[] Export(IEnumerable<ExpenseDto> expenses)
        {
            using var stream = new MemoryStream();

            ParquetSerializer.SerializeAsync(expenses, stream).GetAwaiter().GetResult();

            return stream.ToArray();
        }
    }
}
