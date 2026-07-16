using Portfolio3D.Samples;
using Xunit;

namespace Portfolio3D.EntityFrameworkCore.Applications;

[Collection(Portfolio3DTestConsts.CollectionDefinitionName)]
public class EfCoreSampleAppServiceTests : SampleAppServiceTests<Portfolio3DEntityFrameworkCoreTestModule>
{

}
