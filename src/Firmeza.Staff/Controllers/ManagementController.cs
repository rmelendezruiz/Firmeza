using Firmeza.Application.Management;
using Firmeza.Application.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Firmeza.Staff.Controllers;

[Route("api/management")]
public sealed class ManagementController(
    GetBoard board,
    GetHeadquarters headquarters,
    GetPerformance performance,
    SaveStaffCard fichas,
    SaveTarget targets,
    ListMessages messages,
    SendMessage send) : StaffController
{
    [Authorize(AuthenticationSchemes = SessionScheme.Name, Roles = Roles.ManagementTeam)]
    [HttpGet]
    public async Task<ActionResult<BoardDto>> Get(
        [FromQuery] string? period,
        [FromQuery] Guid? branch,
        CancellationToken ct)
    {
        return Ok(await board.ExecuteAsync(period, branch, ct));
    }

    [Authorize(AuthenticationSchemes = SessionScheme.Name, Roles = Roles.Headquarters)]
    [HttpGet("headquarters")]
    public async Task<ActionResult<HeadquartersDto>> Headquarters([FromQuery] string? period, CancellationToken ct)
    {
        return Ok(await headquarters.ExecuteAsync(period, ct));
    }

    [Authorize(AuthenticationSchemes = SessionScheme.Name, Roles = Roles.ManagementTeam)]
    [HttpPut("cards")]
    public async Task<IActionResult> SaveStaffCard([FromBody] SaveStaffCardCommand command, CancellationToken ct)
    {
        await fichas.ExecuteAsync(command, ct);
        return NoContent();
    }

    [Authorize(AuthenticationSchemes = SessionScheme.Name, Roles = Roles.ManagementTeam)]
    [HttpPut("targets")]
    public async Task<IActionResult> SaveTarget([FromBody] SaveTargetCommand command, CancellationToken ct)
    {
        await targets.ExecuteAsync(command, ct);
        return NoContent();
    }

    [Authorize(AuthenticationSchemes = SessionScheme.Name, Roles = Roles.Salesperson)]
    [HttpGet("performance")]
    public async Task<ActionResult<PerformanceDto>> Performance([FromQuery] string? period, CancellationToken ct)
    {
        return Ok(await performance.ExecuteAsync(period, ct));
    }

    [HttpGet("messages")]
    public async Task<ActionResult<IReadOnlyList<MessageDto>>> Messages([FromQuery] string con, CancellationToken ct)
    {
        return Ok(await messages.ExecuteAsync(con, ct));
    }

    [HttpPost("messages")]
    public async Task<ActionResult<MessageDto>> Send([FromBody] SendMessageCommand command, CancellationToken ct)
    {
        var message = await send.ExecuteAsync(command, ct);
        return Ok(message);
    }
}
