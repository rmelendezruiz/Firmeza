using Firmeza.Application.Staff;
using Firmeza.Application.Errors;
using Firmeza.Application.Identity;
using Firmeza.Application.Ports;
using Firmeza.Application.Branches;
using Firmeza.Domain.Entities;
using Firmeza.Domain.Rules;

namespace Firmeza.Application.Management;

public sealed class ListMessages(StaffSession session, IManagementStore management)
{
    public async Task<IReadOnlyList<MessageDto>> ExecuteAsync(string? con, CancellationToken ct)
    {
        var yo = await session.RequireAsync(ct);
        var otro = Validation.Email(con);
        var items = await management.ListMessagesAsync(yo.Email, otro, ct);
        return items
            .Select(message => new MessageDto(message.Id, message.FromEmail, message.ToEmail, message.Text, message.Date))
            .ToList();
    }
}
