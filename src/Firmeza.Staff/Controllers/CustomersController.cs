using Firmeza.Application.Customers;
using Microsoft.AspNetCore.Mvc;

namespace Firmeza.Staff.Controllers;

[Route("api/customers")]
public sealed class CustomersController(
    ListCustomers list,
    GetCustomer get,
    CreateCustomer create,
    UpdateCustomer update,
    DeleteCustomer remove) : StaffController
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CustomerDto>>> List(CancellationToken ct)
    {
        var items = await list.ExecuteAsync(ct);
        return Ok(items);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CustomerDto>> Get(Guid id, CancellationToken ct)
    {
        var customer = await get.ExecuteAsync(id, ct);
        return Ok(customer);
    }

    [HttpPost]
    public async Task<ActionResult<CustomerDto>> Create([FromBody] SaveCustomerCommand command, CancellationToken ct)
    {
        var customer = await create.ExecuteAsync(command, ct);
        return CreatedAtAction(nameof(Get), new { id = customer.Id }, customer);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<CustomerDto>> Update(
        Guid id,
        [FromBody] SaveCustomerCommand command,
        CancellationToken ct)
    {
        var customer = await update.ExecuteAsync(id, command, ct);
        return Ok(customer);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Remove(Guid id, CancellationToken ct)
    {
        await remove.ExecuteAsync(id, ct);
        return NoContent();
    }
}
