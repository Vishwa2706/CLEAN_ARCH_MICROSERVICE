using System.ComponentModel.DataAnnotations;
using Expense.Application.Commands;
using Expense.Application.Contracts;
using Expense.Application.Factories;
using Expense.Application.Query;
using Expense.Application.Services;
using Expense.Application.Strategies;
using Expense.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.Authorization.Attributes;
using Shared.Authorization.Constants;

namespace Expense.API.Controllers;

[ApiController]
[Route("api/expenses")]
public class ExpenseController : ControllerBase
{
    private readonly ILoggerService _logger;

    private readonly IMediator _mediator;

    private readonly ExpenseExporterFactory _exporterFactory;

    private readonly MonthlyExpenseSummaryStrategy _monthly;
    private readonly CategoryExpenseSummaryStrategy _category;

    private readonly PatchExpenseCommand _patchExpenseCommand;
    private readonly DeleteExpenseCommand _deleteExpenseCommand;

    public ExpenseController(
        IMediator mediator,
        ILoggerService logger,
        ExpenseExporterFactory exporterFactory,
        MonthlyExpenseSummaryStrategy monthly,
        CategoryExpenseSummaryStrategy category,
        PatchExpenseCommand patchExpenseCommand,
        DeleteExpenseCommand deleteExpenseCommand
    )
    {
        _mediator = mediator;
        _logger = logger;
        _exporterFactory = exporterFactory;
        _monthly = monthly;
        _category = category;
        _patchExpenseCommand = patchExpenseCommand;
        _deleteExpenseCommand = deleteExpenseCommand;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery(Name = "search-term")] string? searchTerm = "",
        [FromQuery(Name = "start-index")] int startIndex = 0,
        [FromQuery(Name = "page-size")] int pageSize = 10
    )
    {
        var expenses = await _mediator.Send(
            new GetAllExpenseQuery
            {
                SearchTerm = searchTerm,
                StartIndex = startIndex,
                PageSize = pageSize,
            }
        );

        if (!expenses.Any())
            return NoContent();

        return Ok(expenses);
    }

    [HttpGet("export/{type}")]
    public async Task<IActionResult> ExportExpenses([FromRoute] string type)
    {
        try
        {
            _logger.LogInfo($"Exporting expenses as {type}");

            var expenses = await _mediator.Send(
                new GetAllExpenseQuery { StartIndex = 0, PageSize = 1000 }
            );

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
        var expenses = await _mediator.Send(
            new GetAllExpenseQuery
            {
                SearchTerm = searchTerm,
                StartIndex = startIndex,
                PageSize = pageSize,
            }
        );

        var context = type.ToLower() switch
        {
            "month" => new ExpenseSummaryContext(_monthly),

            "category" => new ExpenseSummaryContext(_category),

            _ => throw new ArgumentException("Invalid summary type"),
        };

        var result = context.Generate(expenses.AsQueryable(), month, category);

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateExpenseRequest request)
    {
        var id = await _mediator.Send(new CreateExpenseCommand(request));

        return Ok(id);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        [FromRoute] int id,
        [FromBody] CreateExpenseRequest request,
        CancellationToken cancellationToken
    )
    {
        var updatedId = await _mediator.Send(
            new UpdateExpenseCommand(id, request),
            cancellationToken
        );

        return Ok(updatedId);
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
