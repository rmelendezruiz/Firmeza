using Firmeza.Application.Cars;
using Firmeza.Application.Catalog;
using Firmeza.Application.Customers;
using Firmeza.Application.Staff;
using Firmeza.Application.Management;
using Firmeza.Application.Metrics;
using Firmeza.Application.Storefront;
using Firmeza.Application.Products;
using Firmeza.Application.Branches;
using Firmeza.Application.Session;
using Firmeza.Application.Sales;
using Microsoft.Extensions.DependencyInjection;

namespace Firmeza.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddSite(this IServiceCollection services)
    {
        services.AddScoped<ListBranches>();
        services.AddScoped<GetBranch>();
        services.AddScoped<ListShowcase>();
        services.AddScoped<SearchParts>();
        services.AddScoped<OpenAccount>();
        services.AddScoped<SignInCustomer>();
        services.AddScoped<SignOut>();
        services.AddScoped<BuyOnSite>();
        services.AddScoped<ListMyPurchases>();
        return services;
    }

    public static IServiceCollection AddStaff(this IServiceCollection services)
    {
        services.AddScoped<StaffSession>();
        services.AddScoped<SignIn>();
        services.AddScoped<GetSession>();
        services.AddScoped<SignOut>();

        services.AddScoped<ListBranches>();
        services.AddScoped<GetBranch>();

        services.AddScoped<ListProducts>();
        services.AddScoped<GetProduct>();
        services.AddScoped<CreateProduct>();
        services.AddScoped<UpdateProduct>();
        services.AddScoped<DeleteProduct>();

        services.AddScoped<ListCars>();
        services.AddScoped<GetCar>();
        services.AddScoped<CreateCar>();
        services.AddScoped<UpdateCar>();
        services.AddScoped<DeleteCar>();

        services.AddScoped<ListAccounts>();
        services.AddScoped<CreateAccount>();
        services.AddScoped<AssignBranch>();

        services.AddScoped<ListBrands>();
        services.AddScoped<GetBrand>();
        services.AddScoped<CreateBrand>();
        services.AddScoped<UpdateBrand>();
        services.AddScoped<DeleteBrand>();

        services.AddScoped<ListCategories>();
        services.AddScoped<GetCategory>();
        services.AddScoped<CreateCategory>();
        services.AddScoped<UpdateCategory>();
        services.AddScoped<DeleteCategory>();

        services.AddScoped<ListVehicles>();
        services.AddScoped<GetVehicle>();
        services.AddScoped<CreateVehicle>();
        services.AddScoped<UpdateVehicle>();
        services.AddScoped<DeleteVehicle>();

        services.AddScoped<ListCustomers>();
        services.AddScoped<GetCustomer>();
        services.AddScoped<CreateCustomer>();
        services.AddScoped<UpdateCustomer>();
        services.AddScoped<DeleteCustomer>();

        services.AddScoped<ListSales>();
        services.AddScoped<GetSale>();
        services.AddScoped<RegisterSale>();
        services.AddScoped<CloseInspection>();
        services.AddScoped<CancelSale>();
        services.AddScoped<ExpireInspections>();
        services.AddScoped<DeleteSale>();

        services.AddScoped<GetDesk>();
        services.AddScoped<GetBoard>();
        services.AddScoped<GetHeadquarters>();
        services.AddScoped<GetPerformance>();
        services.AddScoped<SaveStaffCard>();
        services.AddScoped<SaveTarget>();
        services.AddScoped<ListMessages>();
        services.AddScoped<SendMessage>();
        return services;
    }
}
