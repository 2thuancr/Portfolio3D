using System.Threading.Tasks;
using Portfolio3D.Projects;
using Volo.Abp.Authorization;
using Xunit;

namespace Portfolio3D.EntityFrameworkCore.Applications;

[Collection(Portfolio3DTestConsts.CollectionDefinitionName)]
public class ProjectAppServicePermissionDeniedTests : Portfolio3DApplicationTestBase<ProjectAppServicePermissionDeniedTestModule>
{
    private readonly IProjectAppService _projectAppService;

    public ProjectAppServicePermissionDeniedTests()
    {
        _projectAppService = GetRequiredService<IProjectAppService>();
    }

    [Fact]
    public async Task Should_Deny_Create_Without_Granted_Permission()
    {
        await Assert.ThrowsAsync<AbpAuthorizationException>(
            () => _projectAppService.CreateAsync(new CreateProjectDto
            {
                Name = "Vievent",
                Slug = "vievent",
                Summary = "An event management platform.",
                ThumbnailUrl = "https://example.com/thumbnail.png"
            })
        );
    }
}
