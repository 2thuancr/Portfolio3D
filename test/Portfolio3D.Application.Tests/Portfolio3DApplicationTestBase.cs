using Volo.Abp.Modularity;

namespace Portfolio3D;

public abstract class Portfolio3DApplicationTestBase<TStartupModule> : Portfolio3DTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
