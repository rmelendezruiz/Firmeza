using Firmeza.Application.Staff;
using Firmeza.Application.Errors;
using Firmeza.Application.Identity;
using Firmeza.Application.Ports;
using Firmeza.Application.Branches;
using Firmeza.Domain.Entities;
using Firmeza.Domain.Rules;

namespace Firmeza.Application.Management;

public sealed class SendMessage(
    StaffSession session,
    IStaffAccounts accounts,
    IManagementStore management,
    IBranchStore branches,
    IUnitOfWork work)
{
    public async Task<MessageDto> ExecuteAsync(SendMessageCommand command, CancellationToken ct)
    {
        var yo = await session.RequireAsync(ct);
        var para = Validation.Email(command.ToEmail);
        var roster = await Roster.LoadAsync(accounts, management, branches, ct);
        var destino = roster.Account(para) ?? throw new NotFoundException("Esa cuenta no está en el equipo.");
        if (!roster.CanTalk(yo, destino))
        {
            throw new AccessDeniedException("Ese mensaje no corresponde a tu rol ni a tu counter.");
        }

        var message = StaffMessage.Create(yo.Email, para, command.Text, DateTime.UtcNow);
        await management.AddMessageAsync(message, ct);
        await work.SaveAsync(ct);
        return new MessageDto(message.Id, message.FromEmail, message.ToEmail, message.Text, message.Date);
    }
}
