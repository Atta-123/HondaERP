using HondaERP.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace HondaERP.Api.Controllers;

[ApiController]
[Route("api/branches")]
public class BranchesController : ControllerBase
{
    private readonly IBranchService _service;

    public BranchesController(IBranchService service)
    {
        _service = service;
    }

    //[HttpGet]
    //public async Task<IActionResult> GetAll(CancellationToken ct)
    //{
    //    var branches = await _service.GetAllAsync(ct);

    //    return Ok(branches);
    //}

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(
        int id,
        CancellationToken ct)
    {
        if (id <= 0)
        {
            return BadRequest(new
            {
                message = "Branch ID must be greater than zero."
            });
        }

        var branch = await _service.GetByIdAsync(id, ct);

        if (branch is null)
        {
            return NotFound(new
            {
                message = "Branch not found."
            });
        }

        return Ok(branch);
    }
}