namespace Firmeza.Domain.Entities;

public sealed class Branch
{
    private readonly List<BranchPhoto> _photos = [];
    private readonly List<BranchAsset> _assets = [];

    private Branch()
    {
        Code = "";
        Name = "";
        City = "";
        Headquarters = "";
        Phone = "";
        InspectionPhone = "";
        Email = "";
        Hours = "";
    }

    public Guid Id { get; private set; }

    public string Code { get; private set; }

    public string Name { get; private set; }

    public string City { get; private set; }

    public string Headquarters { get; private set; }

    public string Phone { get; private set; }

    public string InspectionPhone { get; private set; }

    public string Email { get; private set; }

    public string Hours { get; private set; }

    public decimal Latitude { get; private set; }

    public decimal Longitude { get; private set; }

    public int Capacity { get; private set; }

    public int AreaM2 { get; private set; }

    public IReadOnlyCollection<BranchPhoto> Photos => _photos;

    public IReadOnlyCollection<BranchAsset> Assets => _assets;
}
