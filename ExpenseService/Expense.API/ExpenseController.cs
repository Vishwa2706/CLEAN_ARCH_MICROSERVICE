using System.ComponentModel.DataAnnotations;
using Expense.Application.Commands;
using Expense.Application.Contracts;
using Expense.Application.Factories;
using Expense.Application.Query;
using Expense.Application.Services;
using Expense.Application.Strategies;
using Expense.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Shared.Authorization.Attributes;
using Shared.Authorization.Constants;

namespace Expense.API.Controllers;

[ApiController]
[Route("api/expenses")]
public class ExpenseController : ControllerBase
{
    private readonly GetAllExpenseQuery _getAllExpenseQuery;
    private readonly ILoggerService _logger;

    private readonly ExpenseExporterFactory _exporterFactory;

    private readonly MonthlyExpenseSummaryStrategy _monthly;
    private readonly CategoryExpenseSummaryStrategy _category;

    private readonly CreateExpenseCommand _createExpenseCommand;
    private readonly UpdateExpenseCommand _updateExpenseCommand;
    private readonly PatchExpenseCommand _patchExpenseCommand;
    private readonly DeleteExpenseCommand _deleteExpenseCommand;

    public ExpenseController(
        GetAllExpenseQuery getAllExpenseQuery,
        ILoggerService logger,
        ExpenseExporterFactory exporterFactory,
        MonthlyExpenseSummaryStrategy monthly,
        CategoryExpenseSummaryStrategy category,
        CreateExpenseCommand createExpenseCommand,
        UpdateExpenseCommand updateExpenseCommand,
        PatchExpenseCommand patchExpenseCommand,
        DeleteExpenseCommand deleteExpenseCommand
    )
    {
        _getAllExpenseQuery = getAllExpenseQuery;
        _logger = logger;
        _exporterFactory = exporterFactory;
        _monthly = monthly;
        _category = category;
        _createExpenseCommand = createExpenseCommand;
        _updateExpenseCommand = updateExpenseCommand;
        _patchExpenseCommand = patchExpenseCommand;
        _deleteExpenseCommand = deleteExpenseCommand;
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

    [HttpGet("summary/{type}")]
    public async Task<IActionResult> Get(
        [FromRoute] string type,
        [FromQuery] string? month = null,
        [FromQuery] string? category = null,
        [FromQuery(Name = "search-term")] string? searchTerm = "",
        [FromQuery(Name = "start-index")] int startIndex = 0,
        [FromQuery(Name = "page-size")] int pageSize = 100
    )
    {
        var expenses = _getAllExpenseQuery.Execute(startIndex, pageSize, searchTerm);

        var context = type.ToLower() switch
        {
            "month" => new ExpenseSummaryContext(_monthly),
            "category" => new ExpenseSummaryContext(_category),
            _ => throw new ArgumentException("Invalid summary type"),
        };

        var result = context.Generate(expenses, month, category);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateExpenseRequest request)
    {
        try
        {
            var id = await _createExpenseCommand.Execute(request);
            return Ok(id);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        [FromRoute] int id,
        [FromBody] CreateExpenseRequest request
    )
    {
        try
        {
            var updatedId = await _updateExpenseCommand.Execute(id, request);
            return Ok(updatedId);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> Patch(
        [FromRoute] int id,
        [FromBody] PatchExpenseRequest request
    )
    {
        try
        {
            var updatedId = await _patchExpenseCommand.Execute(id, request);
            return Ok(updatedId);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [PermissionAuthorize(Permissions.ExpenseDelete)]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        try
        {
            var deletedId = await _deleteExpenseCommand.Execute(id);
            return Ok(deletedId);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
