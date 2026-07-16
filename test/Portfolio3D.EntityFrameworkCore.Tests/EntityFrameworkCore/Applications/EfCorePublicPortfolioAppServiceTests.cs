using Portfolio3D.PublicPortfolio;
using Xunit;

namespace Portfolio3D.EntityFrameworkCore.Applications;

[Collection(Portfolio3DTestConsts.CollectionDefinitionName)]
public class EfCorePublicPortfolioAppServiceTests : PublicPortfolioAppServiceTests<Portfolio3DEntityFrameworkCoreTestModule>
{
}
