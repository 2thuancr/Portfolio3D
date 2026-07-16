using Portfolio3D.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace Portfolio3D.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(Portfolio3DEntityFrameworkCoreModule),
    typeof(Portfolio3DApplicationContractsModule)
)]
public class Portfolio3DDbMigratorModule : AbpModule
{
}
