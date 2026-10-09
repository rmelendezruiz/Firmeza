namespace Firmeza.Domain.Entities;

public sealed class BranchAsset
{
    private BranchAsset()
    {
        Name = "";
    }

    public Guid Id { get; private set; }

    public Guid BranchId { get; private set; }

    public string Name { get; private set; }

    public AssetType Type { get; private set; }

    public int Quantity { get; private set; }

    public AssetStatus Status { get; private set; }
}
