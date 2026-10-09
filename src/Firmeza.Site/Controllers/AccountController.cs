using Firmeza.Application.Identity;
using Firmeza.Application.Storefront;
using Firmeza.Hosting;
using Firmeza.Application.Session;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Firmeza.Site.Controllers;

[ApiController]
[Route("api/account")]
public sealed class AccountController(
    OpenAccount open,
    SignInCustomer signIn,
    SignOut signOut) : ControllerBase
{
    [EnableRateLimiting(RateLimits.SignIn)]
    [HttpPost]
    public async Task<IActionResult> Open([FromBody] OpenAccountCommand command, CancellationToken ct)
    {
        await open.ExecuteAsync(command, ct);
        return NoContent();
    }

    [EnableRateLimiting(RateLimits.SignIn)]
    [HttpPost("sign-in")]
    public async Task<ActionResult<SessionDto>> SignIn([FromBody] SignInCommand command, CancellationToken ct)
    {
        var session = await signIn.ExecuteAsync(command, ct);
        return Ok(session);
    }

    [Authorize(AuthenticationSchemes = SessionScheme.Name, Roles = Roles.Customer)]
    [HttpPost("sign-out")]
    public async Task<IActionResult> SignOut(CancellationToken ct)
    {
        await signOut.ExecuteAsync(ct);
        return NoContent();
    }
}
