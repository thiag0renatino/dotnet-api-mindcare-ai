using Microsoft.EntityFrameworkCore;
using MindCareAi.Infrastructure.Data;

namespace MindCareAi.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    private const string OracleConnectionName = "OracleDb";

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<MindCareContext>(options =>
        {
            var connectionString = configuration.GetConnectionString(OracleConnectionName)
                                  ?? throw new InvalidOperationException($"Connection string '{OracleConnectionName}' não foi encontrada.");
            options.UseOracle(connectionString, oracleOptions =>
            {
                oracleOptions.UseOracleSQLCompatibility("12");
            });
        });

        services.AddScoped<MindCareContextInitializer>();

        return services;
    }
}
