using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Firmeza.Infrastructure.Persistence;

public sealed class FirmezaDbContextFactory : IDesignTimeDbContextFactory<FirmezaDbContext>
{
    public FirmezaDbContext CreateDbContext(string[] args)
    {
        var directory = FindStaffDirectory();
        var configuration = new ConfigurationBuilder()
            .SetBasePath(directory)
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .Build();

        var connection = configuration.GetConnectionString("Firmeza");
        if (string.IsNullOrWhiteSpace(connection))
        {
            throw new InvalidOperationException("Falta ConnectionStrings:Firmeza.");
        }

        var options = new DbContextOptionsBuilder<FirmezaDbContext>()
            .UseNpgsql(connection)
            .Options;
        return new FirmezaDbContext(options);
    }

    private static string FindStaffDirectory()
    {
        var current = new DirectoryInfo(Directory.GetCurrentDirectory());
        while (current is not null)
        {
            var candidate = Path.Combine(current.FullName, "src", "Firmeza.Staff");
            if (File.Exists(Path.Combine(candidate, "appsettings.json")))
            {
                return candidate;
            }

            if (File.Exists(Path.Combine(current.FullName, "appsettings.json"))
                && current.Name == "Firmeza.Staff")
            {
                return current.FullName;
            }

            current = current.Parent;
        }

        throw new InvalidOperationException("No se encontró el proyecto Firmeza.Staff para leer la cadena de conexión.");
    }
}
