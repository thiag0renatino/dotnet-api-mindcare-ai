using Microsoft.EntityFrameworkCore;

namespace MindCareAi.Infrastructure.Data;

public class MindCareContextInitializer(MindCareContext context, ILogger<MindCareContextInitializer> logger)
{
    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Migrando e validando Oracle database...");
        await context.Database.MigrateAsync(cancellationToken);
        logger.LogInformation("Oracle database migrado/validado com sucesso.");
    }
}
