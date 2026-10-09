using Firmeza.Application.Branches;
using Microsoft.AspNetCore.Mvc;

namespace Firmeza.Site.Controllers;

[ApiController]
[Route("api/branches")]
public sealed class BranchesController(ListBranches list, GetBranch get) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<BranchDto>>> List(CancellationToken ct)
    {
        var items = await list.ExecuteAsync(ct);
        return Ok(items);
    }

    [HttpGet("{code}")]
    public async Task<ActionResult<BranchDto>> Get(string code, CancellationToken ct)
    {
        return Ok(await get.ExecuteAsync(code, ct));
    }
}
