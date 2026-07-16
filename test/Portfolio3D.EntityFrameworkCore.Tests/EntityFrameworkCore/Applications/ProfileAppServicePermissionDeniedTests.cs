using System.Threading.Tasks;
using Portfolio3D.Profiles;
using Volo.Abp.Authorization;
using Xunit;

namespace Portfolio3D.EntityFrameworkCore.Applications;

[Collection(Portfolio3DTestConsts.CollectionDefinitionName)]
public class ProfileAppServicePermissionDeniedTests : Portfolio3DApplicationTestBase<ProjectAppServicePermissionDeniedTestModule>
{
    private readonly IProfileAppService _profileAppService;

    public ProfileAppServicePermissionDeniedTests()
    {
        _profileAppService = GetRequiredService<IProfileAppService>();
    }

    [Fact]
    public async Task Should_Deny_Update_Without_Granted_Permission()
    {
        await Assert.ThrowsAsync<AbpAuthorizationException>(
            () => _profileAppService.UpdateAsync(new UpdateProfileDto
            {
                DisplayName = "Jane Doe",
                Headline = "Software Engineer",
                Bio = "Building things on the web."
            })
        );
    }
}
