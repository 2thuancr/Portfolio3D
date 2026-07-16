using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Portfolio3D.Permissions;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;

namespace Portfolio3D.Skills;

public class SkillAppService : Portfolio3DAppService, ISkillAppService
{
    private readonly IRepository<Skill, Guid> _skillRepository;

    public SkillAppService(IRepository<Skill, Guid> skillRepository)
    {
        _skillRepository = skillRepository;
    }

    [Authorize(Portfolio3DPermissions.Skills.Default)]
    public virtual async Task<SkillDto> GetAsync(Guid id)
    {
        var skill = await _skillRepository.GetAsync(id);
        return ObjectMapper.Map<Skill, SkillDto>(skill);
    }

    [Authorize(Portfolio3DPermissions.Skills.Default)]
    public virtual async Task<PagedResultDto<SkillDto>> GetListAsync(SkillListInput input)
    {
        var queryable = await _skillRepository.GetQueryableAsync();

        queryable = queryable
            .WhereIf(!input.Filter.IsNullOrWhiteSpace(), s =>
                s.Name.Contains(input.Filter!) ||
                s.Category.Contains(input.Filter!))
            .WhereIf(input.IsPublished.HasValue, s => s.IsPublished == input.IsPublished);

        var totalCount = await AsyncExecuter.CountAsync(queryable);

        queryable = queryable
            .OrderBy(input.Sorting.IsNullOrWhiteSpace() ? nameof(Skill.DisplayOrder) : input.Sorting)
            .Skip(input.SkipCount)
            .Take(input.MaxResultCount);

        var skills = await AsyncExecuter.ToListAsync(queryable);

        return new PagedResultDto<SkillDto>(
            totalCount,
            ObjectMapper.Map<List<Skill>, List<SkillDto>>(skills)
        );
    }

    [Authorize(Portfolio3DPermissions.Skills.Create)]
    public virtual async Task<SkillDto> CreateAsync(CreateSkillDto input)
    {
        var skill = new Skill(
            GuidGenerator.Create(),
            input.Name,
            input.Category,
            input.IconUrl,
            input.LevelLabel,
            input.DisplayOrder
        );

        await _skillRepository.InsertAsync(skill);

        return ObjectMapper.Map<Skill, SkillDto>(skill);
    }

    [Authorize(Portfolio3DPermissions.Skills.Update)]
    public virtual async Task<SkillDto> UpdateAsync(Guid id, UpdateSkillDto input)
    {
        var skill = await _skillRepository.GetAsync(id);

        skill.SetName(input.Name);
        skill.SetCategory(input.Category);
        skill.SetIconUrl(input.IconUrl);
        skill.SetLevelLabel(input.LevelLabel);
        skill.SetDisplayOrder(input.DisplayOrder);

        await _skillRepository.UpdateAsync(skill);

        return ObjectMapper.Map<Skill, SkillDto>(skill);
    }

    [Authorize(Portfolio3DPermissions.Skills.Delete)]
    public virtual async Task DeleteAsync(Guid id)
    {
        await _skillRepository.DeleteAsync(id);
    }

    [Authorize(Portfolio3DPermissions.Skills.Publish)]
    public virtual async Task PublishAsync(Guid id)
    {
        var skill = await _skillRepository.GetAsync(id);
        skill.Publish();
        await _skillRepository.UpdateAsync(skill);
    }

    [Authorize(Portfolio3DPermissions.Skills.Publish)]
    public virtual async Task UnpublishAsync(Guid id)
    {
        var skill = await _skillRepository.GetAsync(id);
        skill.Unpublish();
        await _skillRepository.UpdateAsync(skill);
    }

    [Authorize(Portfolio3DPermissions.Skills.Reorder)]
    public virtual async Task ReorderAsync(ReorderSkillsDto input)
    {
        if (input.Items.IsNullOrEmpty())
        {
            throw new BusinessException(Portfolio3DDomainErrorCodes.SkillReorderItemsEmpty);
        }

        if (input.Items.Select(x => x.Id).Distinct().Count() != input.Items.Count)
        {
            throw new BusinessException(Portfolio3DDomainErrorCodes.SkillReorderDuplicateId);
        }

        if (input.Items.Select(x => x.DisplayOrder).Distinct().Count() != input.Items.Count)
        {
            throw new BusinessException(Portfolio3DDomainErrorCodes.SkillReorderDuplicateDisplayOrder);
        }

        var ids = input.Items.Select(x => x.Id).ToList();
        var skills = await _skillRepository.GetListAsync(s => ids.Contains(s.Id));

        if (skills.Count != ids.Count)
        {
            throw new BusinessException(Portfolio3DDomainErrorCodes.SkillReorderSkillNotFound);
        }

        var displayOrderBySkillId = input.Items.ToDictionary(x => x.Id, x => x.DisplayOrder);

        foreach (var skill in skills)
        {
            skill.SetDisplayOrder(displayOrderBySkillId[skill.Id]);
        }

        await _skillRepository.UpdateManyAsync(skills);
    }
}
