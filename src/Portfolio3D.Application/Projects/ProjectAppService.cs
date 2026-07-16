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

namespace Portfolio3D.Projects;

public class ProjectAppService : Portfolio3DAppService, IProjectAppService
{
    private readonly IRepository<Project, Guid> _projectRepository;

    public ProjectAppService(IRepository<Project, Guid> projectRepository)
    {
        _projectRepository = projectRepository;
    }

    [Authorize(Portfolio3DPermissions.Projects.Default)]
    public virtual async Task<ProjectDto> GetAsync(Guid id)
    {
        var project = await _projectRepository.GetAsync(id);
        return ObjectMapper.Map<Project, ProjectDto>(project);
    }

    [Authorize(Portfolio3DPermissions.Projects.Default)]
    public virtual async Task<PagedResultDto<ProjectListDto>> GetListAsync(ProjectListInput input)
    {
        var queryable = await _projectRepository.GetQueryableAsync();

        queryable = queryable
            .WhereIf(!input.Filter.IsNullOrWhiteSpace(), p =>
                p.Name.Contains(input.Filter!) ||
                p.Slug.Contains(input.Filter!) ||
                p.Summary.Contains(input.Filter!))
            .WhereIf(input.IsPublished.HasValue, p => p.IsPublished == input.IsPublished)
            .WhereIf(input.IsFeatured.HasValue, p => p.IsFeatured == input.IsFeatured);

        var totalCount = await AsyncExecuter.CountAsync(queryable);

        queryable = queryable
            .OrderBy(input.Sorting.IsNullOrWhiteSpace() ? nameof(Project.DisplayOrder) : input.Sorting)
            .Skip(input.SkipCount)
            .Take(input.MaxResultCount);

        var projects = await AsyncExecuter.ToListAsync(queryable);

        return new PagedResultDto<ProjectListDto>(
            totalCount,
            ObjectMapper.Map<List<Project>, List<ProjectListDto>>(projects)
        );
    }

    [Authorize(Portfolio3DPermissions.Projects.Create)]
    public virtual async Task<ProjectDto> CreateAsync(CreateProjectDto input)
    {
        await ValidateSlugIsUniqueAsync(input.Slug);

        var project = new Project(
            GuidGenerator.Create(),
            input.Name,
            input.Slug,
            input.Summary,
            input.ThumbnailUrl,
            input.Description,
            input.DemoUrl,
            input.RepositoryUrl,
            input.DisplayOrder
        );

        project.SetFeatured(input.IsFeatured);

        await _projectRepository.InsertAsync(project);

        return ObjectMapper.Map<Project, ProjectDto>(project);
    }

    [Authorize(Portfolio3DPermissions.Projects.Update)]
    public virtual async Task<ProjectDto> UpdateAsync(Guid id, UpdateProjectDto input)
    {
        var project = await _projectRepository.GetAsync(id);

        await ValidateSlugIsUniqueAsync(input.Slug, id);

        project.SetName(input.Name);
        project.SetSlug(input.Slug);
        project.SetSummary(input.Summary);
        project.SetDescription(input.Description);
        project.SetThumbnailUrl(input.ThumbnailUrl);
        project.SetDemoUrl(input.DemoUrl);
        project.SetRepositoryUrl(input.RepositoryUrl);
        project.SetDisplayOrder(input.DisplayOrder);
        project.SetFeatured(input.IsFeatured);

        await _projectRepository.UpdateAsync(project);

        return ObjectMapper.Map<Project, ProjectDto>(project);
    }

    [Authorize(Portfolio3DPermissions.Projects.Delete)]
    public virtual async Task DeleteAsync(Guid id)
    {
        await _projectRepository.DeleteAsync(id);
    }

    [Authorize(Portfolio3DPermissions.Projects.Publish)]
    public virtual async Task PublishAsync(Guid id)
    {
        var project = await _projectRepository.GetAsync(id);
        project.Publish();
        await _projectRepository.UpdateAsync(project);
    }

    [Authorize(Portfolio3DPermissions.Projects.Publish)]
    public virtual async Task UnpublishAsync(Guid id)
    {
        var project = await _projectRepository.GetAsync(id);
        project.Unpublish();
        await _projectRepository.UpdateAsync(project);
    }

    [Authorize(Portfolio3DPermissions.Projects.Reorder)]
    public virtual async Task ReorderAsync(ReorderProjectsDto input)
    {
        if (input.Items.IsNullOrEmpty())
        {
            throw new BusinessException(Portfolio3DDomainErrorCodes.ProjectReorderItemsEmpty);
        }

        if (input.Items.Select(x => x.Id).Distinct().Count() != input.Items.Count)
        {
            throw new BusinessException(Portfolio3DDomainErrorCodes.ProjectReorderDuplicateId);
        }

        if (input.Items.Select(x => x.DisplayOrder).Distinct().Count() != input.Items.Count)
        {
            throw new BusinessException(Portfolio3DDomainErrorCodes.ProjectReorderDuplicateDisplayOrder);
        }

        var ids = input.Items.Select(x => x.Id).ToList();
        var projects = await _projectRepository.GetListAsync(p => ids.Contains(p.Id));

        if (projects.Count != ids.Count)
        {
            throw new BusinessException(Portfolio3DDomainErrorCodes.ProjectReorderProjectNotFound);
        }

        var displayOrderByProjectId = input.Items.ToDictionary(x => x.Id, x => x.DisplayOrder);

        foreach (var project in projects)
        {
            project.SetDisplayOrder(displayOrderByProjectId[project.Id]);
        }

        await _projectRepository.UpdateManyAsync(projects);
    }

    protected virtual async Task ValidateSlugIsUniqueAsync(string slug, Guid? excludedId = null)
    {
        var normalizedSlug = slug.Trim().ToLowerInvariant();
        var queryable = await _projectRepository.GetQueryableAsync();

        var exists = await AsyncExecuter.AnyAsync(
            queryable.Where(p => p.Slug == normalizedSlug && (excludedId == null || p.Id != excludedId))
        );

        if (exists)
        {
            throw new BusinessException(Portfolio3DDomainErrorCodes.ProjectSlugAlreadyExists)
                .WithData("Slug", slug);
        }
    }
}
