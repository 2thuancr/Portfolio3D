using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Portfolio3D.Projects;
using Portfolio3D.Skills;
using Volo.Abp.Domain.Repositories;

namespace Portfolio3D.PublicPortfolio;

[AllowAnonymous]
public class PublicPortfolioAppService : Portfolio3DAppService, IPublicPortfolioAppService
{
    private const int MaxFeaturedProjects = 6;

    private readonly IRepository<Project, Guid> _projectRepository;
    private readonly IRepository<Skill, Guid> _skillRepository;
    private readonly PortfolioProfileOptions _profileOptions;

    public PublicPortfolioAppService(
        IRepository<Project, Guid> projectRepository,
        IRepository<Skill, Guid> skillRepository,
        IOptions<PortfolioProfileOptions> profileOptions)
    {
        _projectRepository = projectRepository;
        _skillRepository = skillRepository;
        _profileOptions = profileOptions.Value;
    }

    public virtual async Task<PublicPortfolioDto> GetAsync()
    {
        return new PublicPortfolioDto
        {
            Profile = MapProfile(),
            FeaturedProjects = await GetFeaturedProjectsAsync(),
            SkillGroups = await GetSkillGroupsAsync()
        };
    }

    private ProfilePublicDto? MapProfile()
    {
        /* No Profile entity exists yet (Task 005 scope explicitly excludes
         * Profile/Skill CRUD). Data is sourced from typed configuration
         * (PortfolioProfileOptions) instead of being hard-coded here.
         * If DisplayName isn't configured, Profile is intentionally null
         * rather than returning an empty/fake object.
         * TODO: replace with a Profile entity + repository read once that
         * aggregate is implemented. */
        if (string.IsNullOrWhiteSpace(_profileOptions.DisplayName))
        {
            return null;
        }

        return new ProfilePublicDto
        {
            DisplayName = _profileOptions.DisplayName,
            Headline = _profileOptions.Headline,
            Bio = _profileOptions.Bio,
            AvatarUrl = _profileOptions.AvatarUrl,
            CvUrl = _profileOptions.CvUrl,
            Email = _profileOptions.Email,
            SocialLinks = _profileOptions.SocialLinks
        };
    }

    private async Task<List<ProjectPublicListDto>> GetFeaturedProjectsAsync()
    {
        var queryable = await _projectRepository.GetQueryableAsync();

        var projects = await AsyncExecuter.ToListAsync(
            queryable
                .Where(p => p.IsPublished && p.IsFeatured)
                .OrderBy(p => p.DisplayOrder)
                .ThenByDescending(p => p.CreationTime)
                .Take(MaxFeaturedProjects)
        );

        return ObjectMapper.Map<List<Project>, List<ProjectPublicListDto>>(projects);
    }

    private async Task<List<SkillGroupPublicDto>> GetSkillGroupsAsync()
    {
        var queryable = await _skillRepository.GetQueryableAsync();

        /* Items are ordered before grouping so that:
         *  - items within a group already follow DisplayOrder ASC, CreationTime ASC;
         *  - groups appear in the order their first (lowest-order) item occurs,
         *    since Enumerable.GroupBy preserves first-occurrence key order.
         * This keeps group order deterministic without hard-coding any category. */
        var skills = await AsyncExecuter.ToListAsync(
            queryable
                .Where(s => s.IsPublished)
                .OrderBy(s => s.DisplayOrder)
                .ThenBy(s => s.CreationTime)
        );

        return skills
            .GroupBy(s => s.Category)
            .Select(group => new SkillGroupPublicDto
            {
                Category = group.Key,
                Items = group
                    .Select(s => new SkillPublicDto
                    {
                        Name = s.Name,
                        IconUrl = s.IconUrl,
                        LevelLabel = s.LevelLabel
                    })
                    .ToList()
            })
            .ToList();
    }
}
