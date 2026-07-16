using System.Linq;
using System.Threading.Tasks;
using Portfolio3D.Projects;
using Shouldly;
using Volo.Abp.Modularity;
using Xunit;

namespace Portfolio3D.PublicPortfolio;

public abstract class PublicPortfolioAppServiceTests<TStartupModule> : Portfolio3DApplicationTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    private readonly IPublicPortfolioAppService _publicPortfolioAppService;
    private readonly IProjectAppService _projectAppService;

    protected PublicPortfolioAppServiceTests()
    {
        _publicPortfolioAppService = GetRequiredService<IPublicPortfolioAppService>();
        _projectAppService = GetRequiredService<IProjectAppService>();
    }

    private static CreateProjectDto CreateValidInput(string slug, int displayOrder = 0, bool isFeatured = false)
    {
        return new CreateProjectDto
        {
            Name = "Vievent",
            Slug = slug,
            Summary = "An event management platform.",
            ThumbnailUrl = "https://example.com/thumbnail.png",
            DisplayOrder = displayOrder,
            IsFeatured = isFeatured
        };
    }

    private async Task<ProjectDto> CreateAndPublishAsync(string slug, int displayOrder = 0, bool isFeatured = true)
    {
        var project = await _projectAppService.CreateAsync(CreateValidInput(slug, displayOrder, isFeatured));
        await _projectAppService.PublishAsync(project.Id);
        return project;
    }

    [Fact]
    public async Task Anonymous_Should_Get_Public_Portfolio()
    {
        var result = await _publicPortfolioAppService.GetAsync();

        result.ShouldNotBeNull();
    }

    [Fact]
    public async Task Profile_Should_Be_Null_When_Not_Configured()
    {
        var result = await _publicPortfolioAppService.GetAsync();

        result.Profile.ShouldBeNull();
    }

    [Fact]
    public async Task SkillGroups_Should_Be_Empty()
    {
        var result = await _publicPortfolioAppService.GetAsync();

        result.SkillGroups.ShouldBeEmpty();
    }

    [Fact]
    public async Task FeaturedProjects_Should_Only_Contain_Published_And_Featured_Projects()
    {
        await CreateAndPublishAsync("aggregate-published-featured", isFeatured: true);
        await CreateAndPublishAsync("aggregate-published-not-featured", isFeatured: false);
        await _projectAppService.CreateAsync(CreateValidInput("aggregate-unpublished-featured", isFeatured: true));

        var result = await _publicPortfolioAppService.GetAsync();

        result.FeaturedProjects.ShouldContain(x => x.Slug == "aggregate-published-featured");
        result.FeaturedProjects.ShouldNotContain(x => x.Slug == "aggregate-published-not-featured");
        result.FeaturedProjects.ShouldNotContain(x => x.Slug == "aggregate-unpublished-featured");
    }

    [Fact]
    public async Task FeaturedProjects_Should_Be_Ordered_By_DisplayOrder()
    {
        await CreateAndPublishAsync("aggregate-order-second", 1);
        await CreateAndPublishAsync("aggregate-order-first", 0);

        var result = await _publicPortfolioAppService.GetAsync();

        var slugs = result.FeaturedProjects.Select(x => x.Slug).ToList();
        slugs.IndexOf("aggregate-order-first").ShouldBeLessThan(slugs.IndexOf("aggregate-order-second"));
    }

    [Fact]
    public async Task FeaturedProjects_Should_Be_Limited_To_Six()
    {
        for (var i = 0; i < 7; i++)
        {
            await CreateAndPublishAsync($"aggregate-limit-{i}", i);
        }

        var result = await _publicPortfolioAppService.GetAsync();

        result.FeaturedProjects.Count.ShouldBe(6);
        result.FeaturedProjects.ShouldNotContain(x => x.Slug == "aggregate-limit-6");
    }
}
