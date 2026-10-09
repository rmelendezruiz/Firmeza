using Firmeza.Application.Staff;
using Firmeza.Application.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Firmeza.Staff.Controllers;

[ApiController]
[Authorize(AuthenticationSchemes = SessionScheme.Name, Roles = Roles.Admin)]
[Route("api/accounts")]
public sealed class AccountsController(ListAccounts list, CreateAccount create, AssignBranch asignar) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<AccountDto>>> List(CancellationToken ct)
    {
        var items = await list.ExecuteAsync(ct);
        return Ok(items);
    }

    [HttpPost]
    public async Task<ActionResult<AccountDto>> Create([FromBody] CreateAccountCommand command, CancellationToken ct)
    {
        var account = await create.ExecuteAsync(command, ct);
        return Ok(account);
    }

    [HttpPut("{id}/sede")]
    public async Task<ActionResult<AccountDto>> AssignBranch(
        string id,
        [FromBody] AssignBranchCommand command,
        CancellationToken ct)
    {
        var account = await asignar.ExecuteAsync(id, command, ct);
        return Ok(account);
    }
}
