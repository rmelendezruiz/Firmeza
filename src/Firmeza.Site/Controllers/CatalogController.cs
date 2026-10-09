using Firmeza.Application.Identity;
using Firmeza.Application.Storefront;
using Firmeza.Hosting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Firmeza.Site.Controllers;

[ApiController]
[Authorize(AuthenticationSchemes = SessionScheme.Name, Roles = Roles.Customer)]
[Route("api/catalog")]
public sealed class CatalogController(
    ListShowcase showcase,
    SearchParts parts,
    BuyOnSite comprar,
    ListMyPurchases purchases) : ControllerBase
{
    [HttpGet("cars")]
    public async Task<ActionResult<IReadOnlyList<PublishedCarDto>>> Cars(CancellationToken ct)
    {
        var items = await showcase.ExecuteAsync(null, ct);
        return Ok(items);
    }

    [HttpGet("parts")]
    public async Task<ActionResult<IReadOnlyList<CatalogPartDto>>> Repuestos(
        [FromQuery] string? search,
        CancellationToken ct)
    {
        var items = await parts.ExecuteAsync(search, ct);
        return Ok(items);
    }

    [HttpGet("purchases")]
    public async Task<ActionResult<IReadOnlyList<SitePurchaseDto>>> Compras(CancellationToken ct)
    {
        var items = await purchases.ExecuteAsync(ct);
        return Ok(items);
    }

    [EnableRateLimiting(RateLimits.Purchase)]
    [HttpPost("purchases")]
    public async Task<ActionResult<SitePurchaseDto>> Comprar(
        [FromBody] BuyOnSiteCommand command,
        CancellationToken ct)
    {
        var purchase = await comprar.ExecuteAsync(command, ct);
        return Ok(purchase);
    }
}
