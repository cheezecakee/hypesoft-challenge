using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Hypesoft.Domain.Repositories;
using Hypesoft.Domain.Services;
using Hypesoft.Infrastructure.Data.Context;
using Hypesoft.Infrastructure.Repositories;
using Hypesoft.Infrastructure.Services;
using Hypesoft.Infrastructure.HealthChecks;
using Serilog;

namespace Hypesoft.Infrastructure
{
    public static class InfrastructureServiceRegistration
    {
        private static readonly string[] HealthCheckTags = ["db", "mongodb"];

        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // MongoDB Configuration
            string connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseMongoDB(connectionString, "HypesoftDB"));

            // Repository Registration
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();

            // Service Registration
            services.AddScoped<IDashboardService, DashboardService>();

            // Health Checks
            services.AddHealthChecks()
                .AddCheck<MongoDbHealthCheck>("mongodb", HealthStatus.Unhealthy, HealthCheckTags);

            // Register MongoDbHealthCheck
            services.AddSingleton(provider => new MongoDbHealthCheck(connectionString));

            // Serilog Configuration
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

            services.AddLogging(builder =>
            {
                builder.AddSerilog(dispose: true);
            });

            return services;
        }
    }
}
