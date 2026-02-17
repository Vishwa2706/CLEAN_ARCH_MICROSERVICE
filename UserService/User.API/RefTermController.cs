using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using User.Application.Contracts;
using User.Application.Services;
using User.Domain.Models;

[ApiController]
[Route("api/refterm")]
public class RefTermController : ControllerBase
{
    private readonly RefTermServices _service;

    public RefTermController(RefTermServices service)
    {
        _service = service;
    }

    [HttpGet("{termType}")]
    public async Task<IActionResult> Get(string termType)
    {
        var data = await _service.GetRefTermsAsync(termType);
        return Ok(data);
    }
}
