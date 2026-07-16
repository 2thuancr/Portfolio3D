using Volo.Abp.Modularity;

namespace Portfolio3D;

/* Inherit from this class for your domain layer tests. */
public abstract class Portfolio3DDomainTestBase<TStartupModule> : Portfolio3DTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
