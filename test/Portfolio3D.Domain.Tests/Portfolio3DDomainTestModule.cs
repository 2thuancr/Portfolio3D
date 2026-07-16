using Volo.Abp.Modularity;

namespace Portfolio3D;

[DependsOn(
    typeof(Portfolio3DDomainModule),
    typeof(Portfolio3DTestBaseModule)
)]
public class Portfolio3DDomainTestModule : AbpModule
{

}
