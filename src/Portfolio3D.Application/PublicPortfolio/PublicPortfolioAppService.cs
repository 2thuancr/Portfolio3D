using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Portfolio3D.Profiles;
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
    private readonly IRepository<Profile, Guid> _profileRepository;

    public PublicPortfolioAppService(
        IRepository<Project, Guid> projectRepository,
        IRepository<Skill, Guid> skillRepository,
        IRepository<Profile, Guid> profileRepository)
    {
        _projectRepository = projectRepository;
        _skillRepository = skillRepository;
        _profileRepository = profileRepository;
    }

    public virtual async Task<PublicPortfolioDto> GetAsync()
    {
        return new PublicPortfolioDto
        {
            Profile = await GetProfileAsync(),
            FeaturedProjects = await GetFeaturedProjectsAsync(),
            SkillGroups = await GetSkillGroupsAsync()
        };
    }

    private async Task<ProfilePublicDto?> GetProfileAsync()
    {
        var queryable = await _profileRepository.GetQueryableAsync();
        var profile = await AsyncExecuter.FirstOrDefaultAsync(queryable);

        if (profile == null)
        {
            return null;
        }

        var socialLinks = SocialLinksJsonSerializer.Deserialize(profile.SocialLinksJson);

        return new ProfilePublicDto
        {
            DisplayName = profile.DisplayName,
            Headline = profile.Headline,
            Bio = profile.Bio,
            AvatarUrl = profile.AvatarUrl,
            CvUrl = profile.CvUrl,
            Email = profile.Email,
            SocialLinks = new[]
                {
                    socialLinks.GitHubUrl,
                    socialLinks.LinkedInUrl,
                    socialLinks.FacebookUrl,
                    socialLinks.WebsiteUrl
                }
                .Where(url => !string.IsNullOrWhiteSpace(url))
                .Select(url => url!)
                .ToList()
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
