namespace Firmeza.Application.Identity;

public static class Roles
{
    public const string Admin = "Admin";

    public const string CompanyManager = "GerenteFirmeza";

    public const string Manager = "Gerente";

    public const string Salesperson = "Comercial";

    public const string Customer = "Cliente";

    public const string Staff = "Admin,GerenteFirmeza,Gerente,Comercial";

    public const string Headquarters = "Admin,GerenteFirmeza";

    public const string ManagementTeam = "Admin,GerenteFirmeza,Gerente";

    public const string Publisher = "GerenteFirmeza,Gerente";

    public const string Seller = "Gerente,Comercial";

    public const string Inspection = "GerenteFirmeza,Gerente,Comercial";

    public static readonly string[] All = [Admin, CompanyManager, Manager, Salesperson, Customer];

    public static readonly (string Previous, string Current)[] Replacements = [("Dueño", CompanyManager), ("Ventas", Salesperson)];

    public static string? FirstStaffRole(IEnumerable<string> roles)
    {
        var items = roles as IReadOnlyCollection<string> ?? roles.ToArray();
        if (items.Contains(Admin) || items.Contains("Administrador"))
        {
            return Admin;
        }

        foreach (var role in new[] { CompanyManager, Manager, Salesperson })
        {
            if (items.Contains(role))
            {
                return role;
            }
        }

        return null;
    }

    public static string? SessionRole(IEnumerable<string> roles)
    {
        var items = roles as IReadOnlyCollection<string> ?? roles.ToArray();
        return FirstStaffRole(items) ?? (items.Contains(Customer) ? Customer : null);
    }

    public static bool IsNewStaffRole(string? role) => role is CompanyManager or Manager or Salesperson;

    public static bool SeesAllBranches(string? role) => role is Admin or CompanyManager;

    public static bool HasBranch(string? role) => role is Manager or Salesperson;
}
