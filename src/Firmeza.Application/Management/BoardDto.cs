using Firmeza.Application.Staff;
using Firmeza.Application.Errors;
using Firmeza.Application.Identity;
using Firmeza.Application.Ports;
using Firmeza.Application.Branches;
using Firmeza.Domain.Entities;
using Firmeza.Domain.Rules;

namespace Firmeza.Application.Management;

public sealed record BoardDto(
    string Period,
    BranchSummaryDto Branch,
    string? Manager,
    int Sales,
    decimal Amount,
    decimal Target,
    decimal Progress,
    int OnSite,
    int AtCounter,
    int InInspection,
    int Available,
    IReadOnlyList<PerformanceRowDto> Rows,
    IReadOnlyList<BranchSummaryDto> Branches,
    IReadOnlyList<ContactDto> Contacts);
