using Portfolio3D.Profiles;
using Xunit;

namespace Portfolio3D.EntityFrameworkCore.Applications;

[Collection(Portfolio3DTestConsts.CollectionDefinitionName)]
public class EfCoreProfileAppServiceTests : ProfileAppServiceTests<Portfolio3DEntityFrameworkCoreTestModule>
{
}
