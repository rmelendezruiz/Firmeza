using Firmeza.Application.Identity;
using Firmeza.Application.Metrics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Firmeza.Staff.Controllers;

[Route("api/desk")]
public sealed class MetricsController(GetDesk get) : StaffController
{
    [Authorize(AuthenticationSchemes = SessionScheme.Name, Roles = Roles.Headquarters)]
    [HttpGet]
    public async Task<ActionResult<DeskDto>> Get(CancellationToken ct)
    {
        var mesa = await get.ExecuteAsync(ct);
        return Ok(mesa);
    }
}
