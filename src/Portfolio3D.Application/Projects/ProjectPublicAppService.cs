using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;

namespace Portfolio3D.Projects;

[AllowAnonymous]
public class ProjectPublicAppService : Portfolio3DAppService, IProjectPublicAppService
{
    private readonly IRepository<Project, Guid> _projectRepository;

    public ProjectPublicAppService(IRepository<Project, Guid> projectRepository)
    {
        _projectRepository = projectRepository;
    }

    public virtual async Task<ListResultDto<ProjectPublicListDto>> GetListAsync()
    {
        var queryable = await _projectRepository.GetQueryableAsync();

        var projects = await AsyncExecuter.ToListAsync(
            queryable
                .Where(p => p.IsPublished)
                .OrderBy(p => p.DisplayOrder)
                .ThenByDescending(p => p.CreationTime)
        );

        return new ListResultDto<ProjectPublicListDto>(
            ObjectMapper.Map<List<Project>, List<ProjectPublicListDto>>(projects)
        );
    }

    public virtual async Task<ProjectPublicDetailDto> GetBySlugAsync(string slug)
    {
        var normalizedSlug = slug.Trim().ToLowerInvariant();

        var queryable = await _projectRepository.GetQueryableAsync();
        var project = await AsyncExecuter.FirstOrDefaultAsync(
            queryable.Where(p => p.Slug == normalizedSlug && p.IsPublished)
        );

        if (project == null)
        {
            throw new EntityNotFoundException(typeof(Project), slug);
        }

        return ObjectMapper.Map<Project, ProjectPublicDetailDto>(project);
    }
}
