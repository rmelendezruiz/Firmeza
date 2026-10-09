using Firmeza.Application.Errors;
using Firmeza.Application.Ports;

namespace Firmeza.Application.Branches;

public sealed class ListBranches(IBranchStore branches, ICarStore cars)
{
    public async Task<IReadOnlyList<BranchDto>> ExecuteAsync(CancellationToken ct)
    {
        var available = (await cars.ListPublishedAsync(ct))
            .GroupBy(car => car.BranchId)
            .ToDictionary(grupo => grupo.Key, grupo => grupo.Count());
        return (await branches.ListAsync(ct))
            .Select(branch => new BranchDto(
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
                available.GetValueOrDefault(branch.Id),
                branch.Photos
                    .OrderBy(foto => foto.Sort)
                    .Select(foto => new BranchPhotoDto(foto.Url, foto.Description))
                    .ToList()))
            .ToList();
    }
}
