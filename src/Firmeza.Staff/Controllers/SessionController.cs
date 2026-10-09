using Firmeza.Application.Identity;
using Firmeza.Application.Session;
using Firmeza.Hosting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Firmeza.Staff.Controllers;

[ApiController]
[Authorize(AuthenticationSchemes = SessionScheme.Name, Roles = Roles.Staff)]
[Route("api/session")]
public sealed class SessionController(SignIn signIn, GetSession get, SignOut signOut) : ControllerBase
{
    [AllowAnonymous]
    [EnableRateLimiting(RateLimits.SignIn)]
    [HttpPost]
    public async Task<ActionResult<SessionDto>> SignInStaff([FromBody] SignInCommand command, CancellationToken ct)
    {
        var session = await signIn.ExecuteAsync(command, ct);
        return Ok(session);
    }

    [HttpGet]
    public async Task<ActionResult<CurrentSessionDto>> Current(CancellationToken ct)
    {
        var session = await get.ExecuteAsync(ct);
        return Ok(session);
    }

    [HttpPost("sign-out")]
    public async Task<IActionResult> SignOut(CancellationToken ct)
    {
        await signOut.ExecuteAsync(ct);
        return NoContent();
    }
}
