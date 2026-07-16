using System.Linq;
using Shouldly;
using Xunit;

namespace Portfolio3D.PublicPortfolio;

public class PublicPortfolioAppServiceContractTests
{
    [Fact]
    public void Should_Only_Expose_A_Single_Read_Method()
    {
        var methodNames = typeof(IPublicPortfolioAppService)
            .GetMethods()
            .Select(m => m.Name)
            .ToList();

        methodNames.ShouldBe(new[] { "GetAsync" });
    }
}
