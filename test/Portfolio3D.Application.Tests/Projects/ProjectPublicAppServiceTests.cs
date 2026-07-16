using System.Linq;
using System.Threading.Tasks;
using Shouldly;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Modularity;
using Xunit;

namespace Portfolio3D.Projects;

public abstract class ProjectPublicAppServiceTests<TStartupModule> : Portfolio3DApplicationTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    private readonly IProjectPublicAppService _projectPublicAppService;
    private readonly IProjectAppService _projectAppService;

    protected ProjectPublicAppServiceTests()
    {
        _projectPublicAppService = GetRequiredService<IProjectPublicAppService>();
        _projectAppService = GetRequiredService<IProjectAppService>();
    }

    private static CreateProjectDto CreateValidInput(string slug, int displayOrder = 0)
    {
        return new CreateProjectDto
        {
            Name = "Vievent",
            Slug = slug,
            Summary = "An event management platform.",
            ThumbnailUrl = "https://example.com/thumbnail.png",
            DisplayOrder = displayOrder
        };
    }

    private async Task<ProjectDto> CreateAndPublishAsync(string slug, int displayOrder = 0)
    {
        var project = await _projectAppService.CreateAsync(CreateValidInput(slug, displayOrder));
        await _projectAppService.PublishAsync(project.Id);
        return project;
    }

    [Fact]
    public async Task Anonymous_Should_Get_Published_List()
    {
        await CreateAndPublishAsync("public-list-1");

        var result = await _projectPublicAppService.GetListAsync();

        result.Items.ShouldContain(x => x.Slug == "public-list-1");
    }

    [Fact]
    public async Task List_Should_Only_Contain_Published_Projects()
    {
        await CreateAndPublishAsync("published-only");
        await _projectAppService.CreateAsync(CreateValidInput("unpublished-hidden"));

        var result = await _projectPublicAppService.GetListAsync();

        result.Items.ShouldContain(x => x.Slug == "published-only");
        result.Items.ShouldNotContain(x => x.Slug == "unpublished-hidden");
    }

    [Fact]
    public async Task List_Should_Be_Ordered_By_DisplayOrder_Then_CreationTime_Descending()
    {
        await CreateAndPublishAsync("order-second", 1);
        await CreateAndPublishAsync("order-first", 0);

        var result = await _projectPublicAppService.GetListAsync();

        var slugs = result.Items.Select(x => x.Slug).ToList();
        slugs.IndexOf("order-first").ShouldBeLessThan(slugs.IndexOf("order-second"));
    }

    [Fact]
    public async Task Should_Get_Detail_By_Slug()
    {
        await CreateAndPublishAsync("detail-target");

        var detail = await _projectPublicAppService.GetBySlugAsync("detail-target");

        detail.Slug.ShouldBe("detail-target");
    }

    [Fact]
    public async Task Should_Normalize_Slug_Before_Query()
    {
        await CreateAndPublishAsync("normalize-target");

        var detail = await _projectPublicAppService.GetBySlugAsync("  NORMALIZE-TARGET  ");

        detail.Slug.ShouldBe("normalize-target");
    }

    [Fact]
    public async Task Should_Throw_NotFound_For_Unpublished_Project()
    {
        var project = await _projectAppService.CreateAsync(CreateValidInput("not-published-detail"));

        await Should.ThrowAsync<EntityNotFoundException>(
            () => _projectPublicAppService.GetBySlugAsync(project.Slug)
        );
    }

    [Fact]
    public async Task Should_Throw_NotFound_For_NonExisting_Slug()
    {
        await Should.ThrowAsync<EntityNotFoundException>(
            () => _projectPublicAppService.GetBySlugAsync("does-not-exist")
        );
    }
}
