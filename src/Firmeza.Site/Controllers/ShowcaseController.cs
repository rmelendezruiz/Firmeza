using Firmeza.Application.Storefront;
using Firmeza.Application.Ports;
using Microsoft.AspNetCore.Mvc;

namespace Firmeza.Site.Controllers;

[ApiController]
[Route("api/showcase")]
public sealed class ShowcaseController(ListShowcase list) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<PublishedCarDto>>> List(
        [FromQuery] string? brand,
        [FromQuery] string? line,
        [FromQuery] string? city,
        [FromQuery] int? yearFrom,
        [FromQuery] int? yearTo,
        [FromQuery] decimal? priceFrom,
        [FromQuery] decimal? priceTo,
        CancellationToken ct)
    {
        var filter = new ShowcaseFilter(brand, line, city, yearFrom, yearTo, priceFrom, priceTo);
        var items = await list.ExecuteAsync(filter, ct);
        return Ok(items);
    }
}
