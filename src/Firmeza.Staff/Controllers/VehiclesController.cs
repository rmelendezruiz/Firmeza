using Firmeza.Application.Catalog;
using Microsoft.AspNetCore.Mvc;

namespace Firmeza.Staff.Controllers;

[Route("api/vehicles")]
public sealed class VehiclesController(
    ListVehicles list,
    GetVehicle get,
    CreateVehicle create,
    UpdateVehicle update,
    DeleteVehicle remove) : StaffController
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<VehicleDto>>> List(CancellationToken ct)
    {
        var items = await list.ExecuteAsync(ct);
        return Ok(items);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<VehicleDto>> Get(Guid id, CancellationToken ct)
    {
        var vehicle = await get.ExecuteAsync(id, ct);
        return Ok(vehicle);
    }

    [HttpPost]
    public async Task<ActionResult<VehicleDto>> Create([FromBody] SaveVehicleCommand command, CancellationToken ct)
    {
        var vehicle = await create.ExecuteAsync(command, ct);
        return CreatedAtAction(nameof(Get), new { id = vehicle.Id }, vehicle);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<VehicleDto>> Update(
        Guid id,
        [FromBody] SaveVehicleCommand command,
        CancellationToken ct)
    {
        var vehicle = await update.ExecuteAsync(id, command, ct);
        return Ok(vehicle);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Remove(Guid id, CancellationToken ct)
    {
        await remove.ExecuteAsync(id, ct);
        return NoContent();
    }
}
