using Firmeza.Application.Cars;
using Firmeza.Application.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Firmeza.Staff.Controllers;

[Route("api/cars")]
public sealed class CarsController(
    ListCars list,
    GetCar get,
    CreateCar create,
    UpdateCar update,
    DeleteCar remove) : StaffController
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CarDto>>> List(CancellationToken ct)
    {
        var items = await list.ExecuteAsync(ct);
        return Ok(items);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CarDto>> Get(Guid id, CancellationToken ct)
    {
        var car = await get.ExecuteAsync(id, ct);
        return Ok(car);
    }

    [Authorize(AuthenticationSchemes = SessionScheme.Name, Roles = Roles.Publisher)]
    [HttpPost]
    public async Task<ActionResult<CarDto>> Create([FromBody] SaveCarCommand command, CancellationToken ct)
    {
        var car = await create.ExecuteAsync(command, ct);
        return CreatedAtAction(nameof(Get), new { id = car.Id }, car);
    }

    [Authorize(AuthenticationSchemes = SessionScheme.Name, Roles = Roles.Publisher)]
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<CarDto>> Update(
        Guid id,
        [FromBody] SaveCarCommand command,
        CancellationToken ct)
    {
        var car = await update.ExecuteAsync(id, command, ct);
        return Ok(car);
    }

    [Authorize(AuthenticationSchemes = SessionScheme.Name, Roles = Roles.Publisher)]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Remove(Guid id, CancellationToken ct)
    {
        await remove.ExecuteAsync(id, ct);
        return NoContent();
    }
}
