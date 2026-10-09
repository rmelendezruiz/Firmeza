namespace Firmeza.Domain.Entities;

public sealed class BranchPhoto
{
    private BranchPhoto()
    {
        Url = "";
        Description = "";
    }

    public Guid Id { get; private set; }

    public Guid BranchId { get; private set; }

    public string Url { get; private set; }

    public string Description { get; private set; }

    public int Sort { get; private set; }
}
