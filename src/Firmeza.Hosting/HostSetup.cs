using Firmeza.Hosting.Errors;
using Firmeza.Infrastructure;
using Firmeza.Infrastructure.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Firmeza.Hosting;

public static class HostSetup
{
    public static WebApplicationBuilder AddHost(this WebApplicationBuilder builder)
    {
        builder.Services.AddInfrastructure(builder.Configuration);
        builder.Services.AddControllers();
        builder.Services.AddOpenApi();
        builder.Services.AddRateLimits();

        var proxyNetwork = builder.Configuration["TrustedProxy:Network"];
        if (!string.IsNullOrWhiteSpace(proxyNetwork))
        {
            builder.Services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
                options.KnownIPNetworks.Add(System.Net.IPNetwork.Parse(proxyNetwork));
            });
        }

        var origins = builder.Configuration.GetSection("AngularOrigins").Get<string[]>();
        if (origins is null || origins.Length == 0)
        {
            origins = [builder.Configuration["AngularOrigin"] ?? "http://localhost:4200"];
        }

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("Angular", policy =>
                policy.WithOrigins(origins).AllowAnyHeader().AllowAnyMethod());
        });

        return builder;
    }

    public static WebApplication UseHost(this WebApplication app)
    {
        if (!string.IsNullOrWhiteSpace(app.Configuration["TrustedProxy:Network"]))
        {
            app.UseForwardedHeaders();
        }

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();
        app.UseCors("Angular");
        app.UseMiddleware<ExceptionMiddleware>();
        app.UseRateLimiter();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        return app;
    }

    public static async Task PrepareSchemaAsync(this WebApplication app)
    {
        await app.Services.ApplySchemaAsync(CancellationToken.None);
        await using var scope = app.Services.CreateAsyncScope();
        var seed = scope.ServiceProvider.GetRequiredService<IdentitySeed>();
        await seed.ExecuteAsync(CancellationToken.None);
    }
}
