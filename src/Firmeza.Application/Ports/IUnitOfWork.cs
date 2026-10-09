namespace Firmeza.Application.Ports;

public interface IUnitOfWork
{
    Task SaveAsync(CancellationToken ct);
}
