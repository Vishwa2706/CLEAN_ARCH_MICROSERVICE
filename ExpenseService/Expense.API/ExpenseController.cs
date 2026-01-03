using Expense.Application.Contracts;
using Expense.Application.Query;
using Microsoft.AspNetCore.Mvc;

namespace Expense.API.Controllers;

[ApiController]
[Route("api/expenses")]
public class ExpenseController : ControllerBase
{
    private readonly GetAllExpenseQuery _getAllExpenseQuery;
    private readonly ILoggerService _logger;

    public ExpenseController(GetAllExpenseQuery getAllExpenseQuery, ILoggerService logger)
    {
        _getAllExpenseQuery = getAllExpenseQuery;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult GetAll(
        [FromQuery(Name = "search-term")] string? searchTerm = "",
        [FromQuery(Name = "start-index")] int startIndex = 0,
        [FromQuery(Name = "page-size")] int pageSize = 10
    )
    {
        try
        {
            _logger.LogInfo("Fetching expenses started");

            var expenses = _getAllExpenseQuery.Execute(startIndex, pageSize, searchTerm).ToList();

            if (!expenses.Any())
            {
                _logger.LogWarning("No expenses found");
                return NoContent();
            }

            _logger.LogInfo($"Fetched {expenses.Count} expenses successfully");
            return Ok(expenses);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error while fetching expenses", ex);
            return StatusCode(500, "Internal server error");
        }
    }
}
