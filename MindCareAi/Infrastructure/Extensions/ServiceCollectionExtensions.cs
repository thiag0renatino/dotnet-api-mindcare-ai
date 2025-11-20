using Microsoft.EntityFrameworkCore;
using MindCareAi.Infrastructure.Data;
using MindCareAi.Services;
using MindCareAi.Services.Interfaces;

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
        services.AddScoped<IAcompanhamentoService, AcompanhamentoService>();
        services.AddScoped<IEncaminhamentoService, EncaminhamentoService>();
        services.AddScoped<ITriagemService, TriagemService>();
        services.AddScoped<IUsuarioService, UsuarioService>();
        services.AddScoped<IEmpresaService, EmpresaService>();
        services.AddScoped<IProfissionalService, ProfissionalService>();

        return services;
    }
}
