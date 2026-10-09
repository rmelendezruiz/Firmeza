using Firmeza.Domain.Rules;

namespace Firmeza.Domain.Entities;

public sealed class StaffCard
{
    private StaffCard()
    {
        Email = "";
    }

    public Guid Id { get; private set; }

    public string Email { get; private set; }

    public Guid? BranchId { get; private set; }

    public decimal Salary { get; private set; }

    public decimal CommissionPercent { get; private set; }

    public static StaffCard Create(string? email, Guid? branchId)
    {
        return new StaffCard
        {
            Id = Guid.NewGuid(),
            Email = Validation.Email(email),
            BranchId = BranchOf(branchId),
        };
    }

    public void Update(decimal salary, decimal commissionPercent)
    {
        Salary = Dinero(salary, "El sueldo");
        CommissionPercent = Porcentaje(commissionPercent);
    }

    public void AssignBranch(Guid? branchId)
    {
        BranchId = BranchOf(branchId);
    }

    private static Guid? BranchOf(Guid? branchId) => branchId == Guid.Empty ? null : branchId;

    private static decimal Dinero(decimal valor, string name)
    {
        if (valor < 0)
        {
            throw new BusinessRuleException($"{name} no puede ser negativo.");
        }

        return decimal.Round(valor, 2, MidpointRounding.AwayFromZero);
    }

    private static decimal Porcentaje(decimal valor)
    {
        if (valor < 0 || valor > 100)
        {
            throw new BusinessRuleException("La comisión tiene que estar entre 0 y 100.");
        }

        return decimal.Round(valor, 2, MidpointRounding.AwayFromZero);
    }
}
