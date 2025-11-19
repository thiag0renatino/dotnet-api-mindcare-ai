using Microsoft.EntityFrameworkCore;

namespace MindCareAi.Infrastructure.Data;

public class MindCareContextInitializer(MindCareContext context, ILogger<MindCareContextInitializer> logger)
{
    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Validando conexão com Oracle database...");
        await context.Database.OpenConnectionAsync(cancellationToken);
        await context.Database.CloseConnectionAsync();
        logger.LogInformation("Oracle database conectado com sucesso.");
    }
}
