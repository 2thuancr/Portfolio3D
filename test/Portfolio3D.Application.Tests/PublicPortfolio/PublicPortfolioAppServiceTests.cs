using System;
using System.Linq;
using System.Threading.Tasks;
using Portfolio3D.Profiles;
using Portfolio3D.Projects;
using Portfolio3D.Skills;
using Shouldly;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Modularity;
using Xunit;

namespace Portfolio3D.PublicPortfolio;

public abstract class PublicPortfolioAppServiceTests<TStartupModule> : Portfolio3DApplicationTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    private readonly IPublicPortfolioAppService _publicPortfolioAppService;
    private readonly IProjectAppService _projectAppService;
    private readonly ISkillAppService _skillAppService;
    private readonly IProfileAppService _profileAppService;
    private readonly IRepository<Profile, Guid> _profileRepository;
    private readonly IRepository<Skill, Guid> _skillRepository;

    protected PublicPortfolioAppServiceTests()
    {
        _publicPortfolioAppService = GetRequiredService<IPublicPortfolioAppService>();
        _projectAppService = GetRequiredService<IProjectAppService>();
        _skillAppService = GetRequiredService<ISkillAppService>();
        _profileAppService = GetRequiredService<IProfileAppService>();
        _profileRepository = GetRequiredService<IRepository<Profile, Guid>>();
        _skillRepository = GetRequiredService<IRepository<Skill, Guid>>();
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

    private async Task<SkillDto> CreateAndPublishSkillAsync(string name, string category, int displayOrder = 0)
    {
        var skill = await _skillAppService.CreateAsync(new CreateSkillDto
        {
            Name = name,
            Category = category,
            DisplayOrder = displayOrder
        });
        await _skillAppService.PublishAsync(skill.Id);
        return skill;
    }

    [Fact]
    public async Task Anonymous_Should_Get_Public_Portfolio()
    {
        var result = await _publicPortfolioAppService.GetAsync();

        result.ShouldNotBeNull();
    }

    [Fact]
    public async Task Profile_Should_Be_Returned_From_Database()
    {
        await _profileAppService.UpdateAsync(new UpdateProfileDto
        {
            DisplayName = "Jane Doe",
            Headline = "Software Engineer",
            Bio = "Building things on the web."
        });

        var result = await _publicPortfolioAppService.GetAsync();

        result.Profile.ShouldNotBeNull();
        result.Profile!.DisplayName.ShouldBe("Jane Doe");
    }

    [Fact]
    public async Task Profile_Should_Be_Null_When_No_Profile_Exists_In_Database()
    {
        var existingProfiles = await _profileRepository.GetListAsync();
        foreach (var profile in existingProfiles)
        {
            await _profileRepository.HardDeleteAsync(profile);
        }

        var result = await _publicPortfolioAppService.GetAsync();

        result.Profile.ShouldBeNull();
    }

    [Fact]
    public async Task SkillGroups_Should_Be_Empty_When_No_Skill_Exists_In_Database()
    {
        var existingSkills = await _skillRepository.GetListAsync();
        foreach (var skill in existingSkills)
        {
            await _skillRepository.HardDeleteAsync(skill);
        }

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

    [Fact]
    public async Task SkillGroups_Should_Only_Contain_Published_Skills()
    {
        await CreateAndPublishSkillAsync("Published Skill", "Backend");
        await _skillAppService.CreateAsync(new CreateSkillDto
        {
            Name = "Unpublished Skill",
            Category = "Backend"
        });

        var result = await _publicPortfolioAppService.GetAsync();

        var names = result.SkillGroups.SelectMany(g => g.Items).Select(i => i.Name).ToList();
        names.ShouldContain("Published Skill");
        names.ShouldNotContain("Unpublished Skill");
    }

    [Fact]
    public async Task SkillGroups_Should_Group_By_Category()
    {
        await CreateAndPublishSkillAsync(".NET", "Backend");
        await CreateAndPublishSkillAsync("Angular", "Frontend");

        var result = await _publicPortfolioAppService.GetAsync();

        result.SkillGroups.ShouldContain(g => g.Category == "Backend" && g.Items.Any(i => i.Name == ".NET"));
        result.SkillGroups.ShouldContain(g => g.Category == "Frontend" && g.Items.Any(i => i.Name == "Angular"));
    }

    [Fact]
    public async Task SkillGroups_Items_Should_Be_Ordered_By_DisplayOrder()
    {
        await CreateAndPublishSkillAsync("Second", "Backend", 1);
        await CreateAndPublishSkillAsync("First", "Backend", 0);

        var result = await _publicPortfolioAppService.GetAsync();

        var backendGroup = result.SkillGroups.Single(g => g.Category == "Backend");
        var names = backendGroup.Items.Select(i => i.Name).ToList();
        names.IndexOf("First").ShouldBeLessThan(names.IndexOf("Second"));
    }
}
