using Portfolio3D.Samples;
using Xunit;

namespace Portfolio3D.EntityFrameworkCore.Domains;

[Collection(Portfolio3DTestConsts.CollectionDefinitionName)]
public class EfCoreSampleDomainTests : SampleDomainTests<Portfolio3DEntityFrameworkCoreTestModule>
{

}
