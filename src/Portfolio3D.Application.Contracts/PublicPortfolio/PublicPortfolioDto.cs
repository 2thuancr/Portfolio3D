using System.Collections.Generic;
using Portfolio3D.Projects;

namespace Portfolio3D.PublicPortfolio;

public class PublicPortfolioDto
{
    public ProfilePublicDto? Profile { get; set; }
    public List<ProjectPublicListDto> FeaturedProjects { get; set; } = new();
    public List<SkillGroupPublicDto> SkillGroups { get; set; } = new();
}
