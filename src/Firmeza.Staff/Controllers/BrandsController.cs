using Firmeza.Application.Catalog;
using Microsoft.AspNetCore.Mvc;

namespace Firmeza.Staff.Controllers;

[Route("api/brands")]
public sealed class BrandsController(
    ListBrands list,
    GetBrand get,
    CreateBrand create,
    UpdateBrand update,
    DeleteBrand remove) : StaffController
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<BrandDto>>> List(CancellationToken ct)
    {
        var items = await list.ExecuteAsync(ct);
        return Ok(items);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<BrandDto>> Get(Guid id, CancellationToken ct)
    {
        var brand = await get.ExecuteAsync(id, ct);
        return Ok(brand);
    }

    [HttpPost]
    public async Task<ActionResult<BrandDto>> Create([FromBody] SaveBrandCommand command, CancellationToken ct)
    {
        var brand = await create.ExecuteAsync(command, ct);
        return CreatedAtAction(nameof(Get), new { id = brand.Id }, brand);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<BrandDto>> Update(
        Guid id,
        [FromBody] SaveBrandCommand command,
        CancellationToken ct)
    {
        var brand = await update.ExecuteAsync(id, command, ct);
        return Ok(brand);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Remove(Guid id, CancellationToken ct)
    {
        await remove.ExecuteAsync(id, ct);
        return NoContent();
    }
}
