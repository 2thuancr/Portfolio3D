using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Portfolio3D.Data;

/* This is used if database provider does't define
 * IPortfolio3DDbSchemaMigrator implementation.
 */
public class NullPortfolio3DDbSchemaMigrator : IPortfolio3DDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}
