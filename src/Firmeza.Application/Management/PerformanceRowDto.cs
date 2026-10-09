using Firmeza.Application.Staff;
using Firmeza.Application.Errors;
using Firmeza.Application.Identity;
using Firmeza.Application.Ports;
using Firmeza.Application.Branches;
using Firmeza.Domain.Entities;
using Firmeza.Domain.Rules;

namespace Firmeza.Application.Management;

public sealed record PerformanceRowDto(
    string Email,
    string? Branch,
    int Sales,
    decimal Amount,
    decimal Target,
    decimal Progress,
    decimal Salary,
    decimal CommissionPercent,
    decimal Commission);
