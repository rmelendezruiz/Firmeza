using Firmeza.Application.Products;
using Firmeza.Application.Ports;
using Microsoft.AspNetCore.Mvc;

namespace Firmeza.Staff.Controllers;

[Route("api/products")]
public sealed class ProductsController(
    ListProducts list,
    GetProduct get,
    CreateProduct create,
    UpdateProduct update,
    DeleteProduct remove) : StaffController
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ProductDto>>> List(
        [FromQuery] string? search,
        [FromQuery] string? existencias,
        CancellationToken ct)
    {
        var filter = existencias switch
        {
            "con" => StockFilter.InStock,
            "sin" => StockFilter.OutOfStock,
            _ => StockFilter.All,
        };
        var items = await list.ExecuteAsync(new ProductQuery(search, filter), ct);
        return Ok(items);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProductDto>> Get(Guid id, CancellationToken ct)
    {
        var product = await get.ExecuteAsync(id, ct);
        return Ok(product);
    }

    [HttpPost]
    public async Task<ActionResult<ProductDto>> Create([FromBody] SaveProductCommand command, CancellationToken ct)
    {
        var product = await create.ExecuteAsync(command, ct);
        return CreatedAtAction(nameof(Get), new { id = product.Id }, product);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ProductDto>> Update(
        Guid id,
        [FromBody] SaveProductCommand command,
        CancellationToken ct)
    {
        var product = await update.ExecuteAsync(id, command, ct);
        return Ok(product);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Remove(Guid id, CancellationToken ct)
    {
        await remove.ExecuteAsync(id, ct);
        return NoContent();
    }
}
