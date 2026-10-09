using Firmeza.Application.Identity;
using Firmeza.Application.Sales;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Firmeza.Staff.Controllers;

[Route("api/sales")]
public sealed class SalesController(
    ListSales list,
    GetSale get,
    RegisterSale register,
    CloseInspection signOut,
    CancelSale cancel,
    DeleteSale remove) : StaffController
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<SaleDto>>> List(CancellationToken ct)
    {
        var items = await list.ExecuteAsync(ct);
        return Ok(items);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<SaleDto>> Get(Guid id, CancellationToken ct)
    {
        var sale = await get.ExecuteAsync(id, ct);
        return Ok(sale);
    }

    [Authorize(AuthenticationSchemes = SessionScheme.Name, Roles = Roles.Seller)]
    [HttpPost]
    public async Task<ActionResult<SaleDto>> Register([FromBody] RegisterSaleCommand command, CancellationToken ct)
    {
        var sale = await register.ExecuteAsync(command, ct);
        return CreatedAtAction(nameof(Get), new { id = sale.Id }, sale);
    }

    [Authorize(AuthenticationSchemes = SessionScheme.Name, Roles = Roles.Inspection)]
    [HttpPost("{id:guid}/close")]
    public async Task<IActionResult> Close(Guid id, CancellationToken ct)
    {
        await signOut.ExecuteAsync(id, ct);
        return NoContent();
    }

    [Authorize(AuthenticationSchemes = SessionScheme.Name, Roles = Roles.Inspection)]
    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid id, CancellationToken ct)
    {
        await cancel.ExecuteAsync(id, ct);
        return NoContent();
    }

    [Authorize(AuthenticationSchemes = SessionScheme.Name, Roles = Roles.Seller)]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Remove(Guid id, CancellationToken ct)
    {
        await remove.ExecuteAsync(id, ct);
        return NoContent();
    }
}
