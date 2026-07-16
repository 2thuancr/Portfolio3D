using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Portfolio3D.Projects;

public interface IProjectAppService : IApplicationService
{
    Task<ProjectDto> GetAsync(Guid id);

    Task<PagedResultDto<ProjectListDto>> GetListAsync(ProjectListInput input);

    Task<ProjectDto> CreateAsync(CreateProjectDto input);

    Task<ProjectDto> UpdateAsync(Guid id, UpdateProjectDto input);

    Task DeleteAsync(Guid id);

    Task PublishAsync(Guid id);

    Task UnpublishAsync(Guid id);

    Task ReorderAsync(ReorderProjectsDto input);
}
