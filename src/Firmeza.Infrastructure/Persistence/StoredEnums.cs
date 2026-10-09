using Firmeza.Domain.Entities;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Firmeza.Infrastructure.Persistence;

public static class StoredEnums
{
    public static readonly ValueConverter<SaleChannel, string> Channel = Map(
        (SaleChannel.PhysicalStore, "PuntoFisico"),
        (SaleChannel.Web, "Pagina"));

    public static readonly ValueConverter<SaleStatus, string> Status = Map(
        (SaleStatus.Closed, "Cerrada"),
        (SaleStatus.Pending, "Preliminar"),
        (SaleStatus.Cancelled, "Anulada"));

    public static readonly ValueConverter<PaymentMethod, string> Pay = Map(
        (PaymentMethod.Cash, "Efectivo"),
        (PaymentMethod.Transfer, "Transferencia"),
        (PaymentMethod.Card, "Tarjeta"),
        (PaymentMethod.Credit, "Credito"));

    public static readonly ValueConverter<CarType, string> Type = Map(
        (CarType.Automobile, "Automovil"),
        (CarType.Truck, "Camion"),
        (CarType.Moto, "Moto"));

    public static readonly ValueConverter<CarCondition, string> Condition = Map(
        (CarCondition.New, "Nuevo"),
        (CarCondition.Used, "Usado"));

    public static readonly ValueConverter<CarAvailability, string> Availability = Map(
        (CarAvailability.SiteOnly, "SoloPagina"),
        (CarAvailability.CounterOnly, "SoloCounter"),
        (CarAvailability.Both, "Ambos"));

    public static readonly ValueConverter<AssetType, string> Asset = Map(
        (AssetType.InfrastructureAsset, "Infraestructura"),
        (AssetType.Equipment, "Equipo"));

    public static readonly ValueConverter<AssetStatus, string> AssetState = Map(
        (AssetStatus.Operational, "Operativo"),
        (AssetStatus.Maintenance, "Mantenimiento"));

    private static ValueConverter<T, string> Map<T>(params (T Value, string Stored)[] pairs)
        where T : struct, Enum
    {
        var to = pairs.ToDictionary(pair => pair.Value, pair => pair.Stored);
        var from = pairs.ToDictionary(pair => pair.Stored, pair => pair.Value);
        return new ValueConverter<T, string>(
            value => to[value],
            stored => from[stored]);
    }
}
