using System;
using System.Linq;
using System.Threading.Tasks;
using Portfolio3D.Profiles;
using Portfolio3D.Projects;
using Portfolio3D.PublicPortfolio;
using Portfolio3D.Skills;
using Shouldly;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;
using Xunit;

namespace Portfolio3D.EntityFrameworkCore.Applications;

[Collection(Portfolio3DTestConsts.CollectionDefinitionName)]
public class PortfolioSeedDataTests : Portfolio3DApplicationTestBase<Portfolio3DEntityFrameworkCoreTestModule>
{
    private readonly IDataSeeder _dataSeeder;
    private readonly IRepository<Profile, Guid> _profileRepository;
    private readonly IRepository<Project, Guid> _projectRepository;
    private readonly IRepository<Skill, Guid> _skillRepository;
    private readonly IProfileAppService _profileAppService;
    private readonly IPublicPortfolioAppService _publicPortfolioAppService;

    public PortfolioSeedDataTests()
    {
        _dataSeeder = GetRequiredService<IDataSeeder>();
        _profileRepository = GetRequiredService<IRepository<Profile, Guid>>();
        _projectRepository = GetRequiredService<IRepository<Project, Guid>>();
        _skillRepository = GetRequiredService<IRepository<Skill, Guid>>();
        _profileAppService = GetRequiredService<IProfileAppService>();
        _publicPortfolioAppService = GetRequiredService<IPublicPortfolioAppService>();
    }

    [Fact]
    public async Task First_Seed_Should_Create_Profile_Project_And_Skills()
    {
        (await _profileRepository.GetCountAsync()).ShouldBe(1);

        var projects = await _projectRepository.GetListAsync(p => p.Slug == "vievent");
        projects.Count.ShouldBe(1);
        projects[0].IsPublished.ShouldBeTrue();
        projects[0].IsFeatured.ShouldBeTrue();

        (await _skillRepository.GetCountAsync()).ShouldBe(9);
    }

    [Fact]
    public async Task Second_Seed_Run_Should_Not_Create_Duplicates()
    {
        await _dataSeeder.SeedAsync();
        await _dataSeeder.SeedAsync();

        (await _profileRepository.GetCountAsync()).ShouldBe(1);

        var projects = await _projectRepository.GetListAsync(p => p.Slug == "vievent");
        projects.Count.ShouldBe(1);

        (await _skillRepository.GetCountAsync()).ShouldBe(9);

        var dotnetSkills = await _skillRepository.GetListAsync(s => s.Name == ".NET" && s.Category == "Backend");
        dotnetSkills.Count.ShouldBe(1);
    }

    [Fact]
    public async Task Existing_Profile_Should_Not_Be_Overwritten_By_ReSeed()
    {
        await _profileAppService.UpdateAsync(new UpdateProfileDto
        {
            DisplayName = "Admin Edited Name",
            Headline = "Admin Edited Headline",
            Bio = "Admin edited bio."
        });

        await _dataSeeder.SeedAsync();

        var profile = await _profileAppService.GetAsync();
        profile.DisplayName.ShouldBe("Admin Edited Name");
    }

    [Fact]
    public async Task PublicPortfolio_Should_Return_Seeded_Data()
    {
        var result = await _publicPortfolioAppService.GetAsync();

        result.Profile.ShouldNotBeNull();
        result.Profile!.DisplayName.ShouldBe("Vi Quốc Thuận");

        result.FeaturedProjects.ShouldContain(p => p.Slug == "vievent");

        result.SkillGroups.ShouldContain(g => g.Category == "Backend" && g.Items.Any(i => i.Name == ".NET"));
    }

    [Fact]
    public async Task PublicPortfolio_SkillGroups_Should_Be_In_Expected_Category_Order()
    {
        var result = await _publicPortfolioAppService.GetAsync();

        var categories = result.SkillGroups.Select(g => g.Category).ToList();

        categories.ShouldBe(new[] { "Backend", "Frontend", "Database", "DevOps/Tools" });
    }

    [Fact]
    public async Task PublicPortfolio_Backend_Skills_Should_Be_In_Expected_Item_Order()
    {
        var result = await _publicPortfolioAppService.GetAsync();

        var backendSkills = result.SkillGroups.Single(g => g.Category == "Backend").Items.Select(i => i.Name).ToList();

        backendSkills.ShouldBe(new[] { ".NET", "ASP.NET Core", "ABP Framework" });
    }
}
