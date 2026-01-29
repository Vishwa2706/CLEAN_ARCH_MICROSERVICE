using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using User.Application.Contracts;
using User.Application.Query;
using User.Domain.Models;

namespace User.API.Controllers;

[ApiController]
[Route("api/users")]
public class UserController : ControllerBase
{
    private readonly GetAllUserQuery _getAllUserQuery;
    private readonly GetUserExpensesQuery _getUserExpensesQuery;
    private readonly GetFamilyAdminService _getFamilyAdminService;

    public UserController(
        GetAllUserQuery getAllUserQuery,
        GetUserExpensesQuery getUserExpensesQuery,
        GetFamilyAdminService getFamilyAdminService
    )
    {
        _getAllUserQuery = getAllUserQuery;
        _getUserExpensesQuery = getUserExpensesQuery;
        _getFamilyAdminService = getFamilyAdminService;
    }

    [HttpGet]
    public IActionResult GetAllUser()
    {
        var users = _getAllUserQuery.Execute().ToList();

        return Ok(users);
    }

    [HttpGet("{userId}/expenses")]
    public async Task<IActionResult> GetUserWithExpenses([FromRoute] int userId)
    {
        var result = await _getUserExpensesQuery.ExecuteAsync(userId);

        if (result == null)
            return NotFound("User not found");

        return Ok(result);
    }

    [HttpGet("{userId}/permissions")]
    public async Task<IActionResult> GetUserPermissionAsync([FromRoute] int userId)
    {
        var result = await _getFamilyAdminService.Execute(userId);

        return Ok(result);
    }
}
