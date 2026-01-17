using System.Globalization;
using CsvHelper;
using Expense.Domain.Models;
using Expense.Infrastructure.Persistence.Seed.CsvModels;
using Expense.Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;

namespace Expense.Infrastructure.Persistence.Seed
{
    public class DatabaseSeeder
    {
        private readonly ExpenseRepository _context;

        public DatabaseSeeder(ExpenseRepository context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            await SeedExpensesAsync();
        }

        private async Task SeedExpensesAsync()
        {
            if (await _context.Expenses.AnyAsync())
                return;

            var path = Path.Combine(
                AppContext.BaseDirectory,
                "Persistence",
                "Seed",
                "Csv",
                "expenses.csv"
            );

            using var reader = new StreamReader(path);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            var expenses = csv.GetRecords<ExpenseCsv>()
                .Select(e => new ExpenseDto
                {
                    UserId = e.UserId,
                    Category = e.Category,
                    Amount = e.Amount,
                    Date = DateTime.SpecifyKind(e.Date, DateTimeKind.Utc),
                    Note = e.Note,
                })
                .ToList();

            _context.Expenses.AddRange(expenses);
            await _context.SaveChangesAsync();
        }
    }
}
