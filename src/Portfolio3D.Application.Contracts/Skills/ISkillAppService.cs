using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Portfolio3D.Skills;

public interface ISkillAppService : IApplicationService
{
    Task<SkillDto> GetAsync(Guid id);

    Task<PagedResultDto<SkillDto>> GetListAsync(SkillListInput input);

    Task<SkillDto> CreateAsync(CreateSkillDto input);

    Task<SkillDto> UpdateAsync(Guid id, UpdateSkillDto input);

    Task DeleteAsync(Guid id);

    Task PublishAsync(Guid id);

    Task UnpublishAsync(Guid id);

    Task ReorderAsync(ReorderSkillsDto input);
}
