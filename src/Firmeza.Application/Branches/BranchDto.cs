using Firmeza.Domain.Entities;

namespace Firmeza.Application.Branches;

public sealed record BranchPhotoDto(string Url, string Description);

public sealed record BranchDto(
    Guid Id,
    string Code,
    string Name,
    string City,
    string Headquarters,
    string Phone,
    string InspectionPhone,
    string Email,
    string Hours,
    decimal Latitude,
    decimal Longitude,
    int Available,
    IReadOnlyList<BranchPhotoDto> Photos);

public sealed record BranchSummaryDto(Guid Id, string Code, string Name, string City)
{
    public static BranchSummaryDto From(Branch branch) => new(branch.Id, branch.Code, branch.Name, branch.City);
}
