using Xunit;

namespace Portfolio3D.EntityFrameworkCore;

[CollectionDefinition(Portfolio3DTestConsts.CollectionDefinitionName)]
public class Portfolio3DEntityFrameworkCoreCollection : ICollectionFixture<Portfolio3DEntityFrameworkCoreFixture>
{

}
