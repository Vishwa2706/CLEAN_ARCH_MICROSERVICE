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

    public UserController(GetAllUserQuery getAllUserQuery)
    {
        _getAllUserQuery = getAllUserQuery;
    }

    [HttpGet]
    public IActionResult GetAllUser()
    {
        var users = _getAllUserQuery.Execute().ToList();

        return Ok(users);
    }
}
