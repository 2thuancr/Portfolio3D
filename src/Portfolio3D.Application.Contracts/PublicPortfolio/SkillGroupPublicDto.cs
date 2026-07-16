using System.Collections.Generic;

namespace Portfolio3D.PublicPortfolio;

public class SkillGroupPublicDto
{
    public string Category { get; set; } = null!;
    public List<SkillPublicDto> Items { get; set; } = new();
}
