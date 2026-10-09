using Firmeza.Application.Staff;
using Firmeza.Application.Errors;
using Firmeza.Application.Ports;
using Firmeza.Domain.Rules;

namespace Firmeza.Application.Session;

public sealed record CurrentSessionDto(string Email, string Role, Guid? BranchId, string? Branch);
