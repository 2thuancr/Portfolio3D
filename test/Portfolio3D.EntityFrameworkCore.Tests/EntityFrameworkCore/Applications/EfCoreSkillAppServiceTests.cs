using Portfolio3D.Skills;
using Xunit;

namespace Portfolio3D.EntityFrameworkCore.Applications;

[Collection(Portfolio3DTestConsts.CollectionDefinitionName)]
public class EfCoreSkillAppServiceTests : SkillAppServiceTests<Portfolio3DEntityFrameworkCoreTestModule>
{
}
