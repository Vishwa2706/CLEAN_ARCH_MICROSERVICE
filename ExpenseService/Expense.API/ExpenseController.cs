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
    public IActionResult GetAll()
    {
        var expenses = _getAllExpenseQuery.Execute().ToList();
        return Ok(expenses);
    }
}
