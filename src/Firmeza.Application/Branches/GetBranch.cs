using Firmeza.Application.Errors;
using Firmeza.Application.Ports;

namespace Firmeza.Application.Branches;

public sealed class GetBranch(IBranchStore branches, ICarStore cars)
{
    public async Task<BranchDto> ExecuteAsync(string? code, CancellationToken ct)
    {
        var trimmed = code?.Trim().ToUpperInvariant() ?? "";
        if (trimmed.Length is 0 or > 8)
        {
            throw new NotFoundException("Counter no encontrado.");
        }

        var branch = await branches.GetByCodeAsync(trimmed, ct)
            ?? throw new NotFoundException("Counter no encontrado.");
        var available = (await cars.ListPublishedAsync(ct)).Count(car => car.BranchId == branch.Id);
        return new BranchDto(
            branch.Id,
            branch.Code,
            branch.Name,
            branch.City,
            branch.Headquarters,
            branch.Phone,
            branch.InspectionPhone,
            branch.Email,
            branch.Hours,
            branch.Latitude,
            branch.Longitude,
            available,
            branch.Photos
                .OrderBy(foto => foto.Sort)
                .Select(foto => new BranchPhotoDto(foto.Url, foto.Description))
                .ToList());
    }
}
