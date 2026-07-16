using System.Threading.Tasks;
using Portfolio3D.Skills;
using Volo.Abp.Authorization;
using Xunit;

namespace Portfolio3D.EntityFrameworkCore.Applications;

[Collection(Portfolio3DTestConsts.CollectionDefinitionName)]
public class SkillAppServicePermissionDeniedTests : Portfolio3DApplicationTestBase<ProjectAppServicePermissionDeniedTestModule>
{
    private readonly ISkillAppService _skillAppService;

    public SkillAppServicePermissionDeniedTests()
    {
        _skillAppService = GetRequiredService<ISkillAppService>();
    }

    [Fact]
    public async Task Should_Deny_Create_Without_Granted_Permission()
    {
        await Assert.ThrowsAsync<AbpAuthorizationException>(
            () => _skillAppService.CreateAsync(new CreateSkillDto
            {
                Name = ".NET",
                Category = "Backend"
            })
        );
    }
}
