using Volo.Abp.Modularity;

namespace Portfolio3D;

[DependsOn(
    typeof(Portfolio3DApplicationModule),
    typeof(Portfolio3DDomainTestModule)
)]
public class Portfolio3DApplicationTestModule : AbpModule
{

}
