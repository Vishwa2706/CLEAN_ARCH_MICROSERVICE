using Expense.Application.Contracts;

namespace Expense.Application.Factories
{
    public class ExpenseExporterFactory
    {
        private readonly IEnumerable<IExpenseExporter> _exporters;

        public ExpenseExporterFactory(IEnumerable<IExpenseExporter> exporters)
        {
            _exporters = exporters;
        }

        public IExpenseExporter Create(string type)
        {
            return _exporters.FirstOrDefault(e =>
                    e.FileExtension.Equals(type, StringComparison.OrdinalIgnoreCase)
                ) ?? throw new ArgumentException("Invalid export type");
        }
    }
}
