using Firmeza.Application.Ports;
using Firmeza.Infrastructure.Identity;
using Firmeza.Infrastructure.Persistence;
using Firmeza.Infrastructure.Persistence.Stores;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QuestPDF.Infrastructure;

namespace Firmeza.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connection = configuration.GetConnectionString("Firmeza");
        if (string.IsNullOrWhiteSpace(connection))
        {
            throw new InvalidOperationException("Falta ConnectionStrings:Firmeza.");
        }

        services.AddDbContext<FirmezaDbContext>(options => options.UseNpgsql(connection));
        services.AddIdentity<IdentityUser, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Lockout.AllowedForNewUsers = true;
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
            })
            .AddEntityFrameworkStores<FirmezaDbContext>();

        services.AddMemoryCache();
        services.AddHttpContextAccessor();
        services.AddSingleton<SessionCache>();
        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = SessionProtector.Scheme;
                options.DefaultChallengeScheme = SessionProtector.Scheme;
            })
            .AddScheme<AuthenticationSchemeOptions, SessionAuthHandler>(SessionProtector.Scheme, _ => { });
        services.AddAuthorization();

        services.AddScoped<IProductStore, ProductStore>();
        services.AddScoped<ICarStore, CarStore>();
        services.AddScoped<IBranchStore, BranchStore>();
        services.AddScoped<IStaffAccounts, StaffAccounts>();
        services.AddScoped<ICustomerAccounts, CustomerAccounts>();
        services.AddScoped<IBrandStore, BrandStore>();
        services.AddScoped<ICategoryStore, CategoryStore>();
        services.AddScoped<IVehicleStore, VehicleStore>();
        services.AddScoped<ICustomerStore, CustomerStore>();
        services.AddScoped<ISaleStore, SaleStore>();
        services.AddScoped<IManagementStore, ManagementStore>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IAdminIdentity, AdminIdentity>();
        services.AddScoped<IdentitySeed>();

        global::QuestPDF.Settings.License = LicenseType.Community;
        return services;
    }

    public static async Task ApplySchemaAsync(this IServiceProvider services, CancellationToken ct)
    {
        await using var scope = services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<FirmezaDbContext>();
        await db.Database.MigrateAsync(ct);
    }
}
