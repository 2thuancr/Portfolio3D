using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Portfolio3D.Projects;

public interface IProjectPublicAppService : IApplicationService
{
    Task<ListResultDto<ProjectPublicListDto>> GetListAsync();

    Task<ProjectPublicDetailDto> GetBySlugAsync(string slug);
}
