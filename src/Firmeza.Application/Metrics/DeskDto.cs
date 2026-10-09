using Firmeza.Application.Ports;
using Firmeza.Application.Sales;
using Firmeza.Domain.Entities;

namespace Firmeza.Application.Metrics;

public sealed record DeskDto(
    int PhysicalStore,
    int Web,
    int Published,
    int Available,
    IReadOnlyList<SaleDto> Recent);
