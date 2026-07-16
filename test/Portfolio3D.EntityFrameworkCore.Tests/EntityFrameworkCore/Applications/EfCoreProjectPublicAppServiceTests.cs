using Portfolio3D.Projects;
using Xunit;

namespace Portfolio3D.EntityFrameworkCore.Applications;

[Collection(Portfolio3DTestConsts.CollectionDefinitionName)]
public class EfCoreProjectPublicAppServiceTests : ProjectPublicAppServiceTests<Portfolio3DEntityFrameworkCoreTestModule>
{
}
