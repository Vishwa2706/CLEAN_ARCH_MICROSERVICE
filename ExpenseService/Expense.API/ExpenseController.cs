using System.ComponentModel.DataAnnotations;
using Expense.Application.Contracts;
using Expense.Application.Factories;
using Expense.Application.Query;
using Microsoft.AspNetCore.Mvc;

namespace Expense.API.Controllers;

[ApiController]
[Route("api/expenses")]
public class ExpenseController : ControllerBase
{
    private readonly GetAllExpenseQuery _getAllExpenseQuery;
    private readonly ILoggerService _logger;

    private readonly ExpenseExporterFactory _exporterFactory;

    public ExpenseController(
        GetAllExpenseQuery getAllExpenseQuery,
        ILoggerService logger,
        ExpenseExporterFactory exporterFactory
    )
    {
        _getAllExpenseQuery = getAllExpenseQuery;
        _logger = logger;
        _exporterFactory = exporterFactory;
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

    [HttpGet("export/{type}")]
    public IActionResult ExportExpenses([FromRoute] string type)
    {
        try
        {
            _logger.LogInfo($"Exporting expenses as {type}");

            var expenses = _getAllExpenseQuery.Execute(0, 1000).ToList();

            var exporter = _exporterFactory.Create(type);
            var fileBytes = exporter.Export(expenses);

            return File(fileBytes, exporter.ContentType, $"expenses.{exporter.FileExtension}");
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex.Message);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error while exporting expenses", ex);
            return StatusCode(500, "Internal server error");
        }
    }
}
