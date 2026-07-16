using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Portfolio3D.Data;
using Volo.Abp.DependencyInjection;

namespace Portfolio3D.EntityFrameworkCore;

public class EntityFrameworkCorePortfolio3DDbSchemaMigrator
    : IPortfolio3DDbSchemaMigrator, ITransientDependency
{
    private readonly IServiceProvider _serviceProvider;

    public EntityFrameworkCorePortfolio3DDbSchemaMigrator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task MigrateAsync()
    {
        /* We intentionally resolving the Portfolio3DDbContext
         * from IServiceProvider (instead of directly injecting it)
         * to properly get the connection string of the current tenant in the
         * current scope.
         */

        await _serviceProvider
            .GetRequiredService<Portfolio3DDbContext>()
            .Database
            .MigrateAsync();
    }
}
