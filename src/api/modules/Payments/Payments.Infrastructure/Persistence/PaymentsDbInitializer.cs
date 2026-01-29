using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.Payments.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.Payments.Infrastructure.Persistence;

internal sealed class PaymentsDbInitializer(
    ILogger<PaymentsDbInitializer> logger,
    PaymentsDbContext context) : IDbInitializer
{
    public async Task MigrateAsync(CancellationToken cancellationToken)
    {
        if ((await context.Database.GetPendingMigrationsAsync(cancellationToken)).Any())
        {
            await context.Database.MigrateAsync(cancellationToken).ConfigureAwait(false);
            logger.LogInformation("[{Tenant}] applied database migrations for payments module", context.TenantInfo!.Identifier);
        }
    }

    public async Task SeedAsync(CancellationToken cancellationToken)
    {
        // Add seed data here if needed
        await Task.CompletedTask;
    }
}
