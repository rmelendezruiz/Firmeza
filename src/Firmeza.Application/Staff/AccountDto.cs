using Firmeza.Application.Errors;
using Firmeza.Application.Identity;
using Firmeza.Application.Ports;
using Firmeza.Domain.Entities;
using Firmeza.Domain.Rules;

namespace Firmeza.Application.Staff;

public sealed record AccountDto(string Id, string Email, string Role, Guid? BranchId, string? Branch);
