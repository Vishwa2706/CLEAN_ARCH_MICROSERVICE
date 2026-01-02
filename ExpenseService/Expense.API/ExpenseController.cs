using Expense.Application.Query;
using Microsoft.AspNetCore.Mvc;

namespace Expense.API.Controllers;

[ApiController]
[Route("api/expenses")]
public class ExpenseController : ControllerBase
{
    private readonly GetAllExpenseQuery _getAllExpenseQuery;

    public ExpenseController(GetAllExpenseQuery getAllExpenseQuery)
    {
        _getAllExpenseQuery = getAllExpenseQuery;
    }

    [HttpGet]
    public IActionResult GetAll(
        [FromQuery(Name = "search-term")] string? searchTerm = "",
        [FromQuery(Name = "start-index")] int startIndex = 0,
        [FromQuery(Name = "page-size")] int pageSize = 10
    )
    {
        var expenses = _getAllExpenseQuery.Execute( startIndex, pageSize, searchTerm).ToList();

        if (expenses.Count == 0)
            return NoContent();

        return Ok(expenses);
    }
}
