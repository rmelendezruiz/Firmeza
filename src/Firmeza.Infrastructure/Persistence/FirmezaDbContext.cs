using Firmeza.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Firmeza.Infrastructure.Persistence;

public sealed class FirmezaDbContext(DbContextOptions<FirmezaDbContext> options)
    : IdentityDbContext<IdentityUser, IdentityRole, string>(options)
{
    public DbSet<Product> Products => Set<Product>();

    public DbSet<Brand> Brands => Set<Brand>();

    public DbSet<Category> Categories => Set<Category>();

    public DbSet<Vehicle> Vehicles => Set<Vehicle>();

    public DbSet<Fitment> Fitments => Set<Fitment>();

    public DbSet<Car> Cars => Set<Car>();

    public DbSet<Customer> Customers => Set<Customer>();

    public DbSet<Sale> Sales => Set<Sale>();

    public DbSet<SaleLine> SaleLines => Set<SaleLine>();

    public DbSet<Branch> Branches => Set<Branch>();

    public DbSet<StaffCard> StaffCards => Set<StaffCard>();

    public DbSet<SalesTarget> Targets => Set<SalesTarget>();

    public DbSet<StaffMessage> Messages => Set<StaffMessage>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(FirmezaDbContext).Assembly);
    }
}
