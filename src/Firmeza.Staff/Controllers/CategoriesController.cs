using Firmeza.Application.Catalog;
using Microsoft.AspNetCore.Mvc;

namespace Firmeza.Staff.Controllers;

[Route("api/categories")]
public sealed class CategoriesController(
    ListCategories list,
    GetCategory get,
    CreateCategory create,
    UpdateCategory update,
    DeleteCategory remove) : StaffController
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CategoryDto>>> List(CancellationToken ct)
    {
        var items = await list.ExecuteAsync(ct);
        return Ok(items);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CategoryDto>> Get(Guid id, CancellationToken ct)
    {
        var category = await get.ExecuteAsync(id, ct);
        return Ok(category);
    }

    [HttpPost]
    public async Task<ActionResult<CategoryDto>> Create([FromBody] SaveCategoryCommand command, CancellationToken ct)
    {
        var category = await create.ExecuteAsync(command, ct);
        return CreatedAtAction(nameof(Get), new { id = category.Id }, category);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<CategoryDto>> Update(
        Guid id,
        [FromBody] SaveCategoryCommand command,
        CancellationToken ct)
    {
        var category = await update.ExecuteAsync(id, command, ct);
        return Ok(category);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Remove(Guid id, CancellationToken ct)
    {
        await remove.ExecuteAsync(id, ct);
        return NoContent();
    }
}
