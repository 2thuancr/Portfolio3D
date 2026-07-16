using System;
using System.Threading.Tasks;
using Shouldly;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Modularity;
using Volo.Abp.Validation;
using Xunit;

namespace Portfolio3D.Profiles;

public abstract class ProfileAppServiceTests<TStartupModule> : Portfolio3DApplicationTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    private readonly IProfileAppService _profileAppService;
    private readonly IRepository<Profile, Guid> _profileRepository;

    protected ProfileAppServiceTests()
    {
        _profileAppService = GetRequiredService<IProfileAppService>();
        _profileRepository = GetRequiredService<IRepository<Profile, Guid>>();
    }

    private static UpdateProfileDto CreateValidInput(string displayName = "Jane Doe")
    {
        return new UpdateProfileDto
        {
            DisplayName = displayName,
            Headline = "Software Engineer",
            Bio = "Building things on the web."
        };
    }

    [Fact]
    public async Task Should_Get_The_Seeded_Profile()
    {
        var profile = await _profileAppService.GetAsync();

        profile.ShouldNotBeNull();
        profile.DisplayName.ShouldNotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Should_Update_The_Existing_Profile_Without_Creating_A_New_One()
    {
        await _profileAppService.UpdateAsync(CreateValidInput("Updated Name"));

        var updated = await _profileAppService.GetAsync();
        updated.DisplayName.ShouldBe("Updated Name");

        (await _profileRepository.GetCountAsync()).ShouldBe(1);
    }

    [Fact]
    public async Task Should_Update_SocialLinks()
    {
        var input = CreateValidInput();
        input.SocialLinks.GitHubUrl = "https://github.com/example";
        input.SocialLinks.WebsiteUrl = "https://example.com";

        var result = await _profileAppService.UpdateAsync(input);

        result.SocialLinks.GitHubUrl.ShouldBe("https://github.com/example");
        result.SocialLinks.WebsiteUrl.ShouldBe("https://example.com");
        result.SocialLinks.LinkedInUrl.ShouldBeNull();
    }

    [Fact]
    public async Task Should_Create_Profile_When_None_Exists()
    {
        var existing = await _profileRepository.GetListAsync();
        foreach (var profile in existing)
        {
            await _profileRepository.HardDeleteAsync(profile);
        }

        (await _profileRepository.GetCountAsync()).ShouldBe(0);

        await _profileAppService.UpdateAsync(CreateValidInput("Brand New"));

        (await _profileRepository.GetCountAsync()).ShouldBe(1);
        (await _profileAppService.GetAsync()).DisplayName.ShouldBe("Brand New");
    }

    [Fact]
    public async Task Should_Only_Ever_Have_One_Active_Profile()
    {
        await _profileAppService.UpdateAsync(CreateValidInput("First Update"));
        await _profileAppService.UpdateAsync(CreateValidInput("Second Update"));

        (await _profileRepository.GetCountAsync()).ShouldBe(1);
    }

    [Fact]
    public async Task Should_Reject_Empty_DisplayName()
    {
        var input = CreateValidInput();
        input.DisplayName = "";

        await Should.ThrowAsync<AbpValidationException>(() => _profileAppService.UpdateAsync(input));
    }

    [Fact]
    public async Task Should_Reject_Empty_Headline()
    {
        var input = CreateValidInput();
        input.Headline = "";

        await Should.ThrowAsync<AbpValidationException>(() => _profileAppService.UpdateAsync(input));
    }

    [Fact]
    public async Task Should_Reject_Empty_Bio()
    {
        var input = CreateValidInput();
        input.Bio = "";

        await Should.ThrowAsync<AbpValidationException>(() => _profileAppService.UpdateAsync(input));
    }

    [Fact]
    public async Task Should_Reject_Invalid_AvatarUrl()
    {
        var input = CreateValidInput();
        input.AvatarUrl = "not-a-url";

        await Should.ThrowAsync<AbpValidationException>(() => _profileAppService.UpdateAsync(input));
    }

    [Fact]
    public async Task Should_Reject_Invalid_Email()
    {
        var input = CreateValidInput();
        input.Email = "not-an-email";

        await Should.ThrowAsync<AbpValidationException>(() => _profileAppService.UpdateAsync(input));
    }
}
