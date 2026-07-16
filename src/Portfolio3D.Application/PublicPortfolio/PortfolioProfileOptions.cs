using System.Collections.Generic;

namespace Portfolio3D.PublicPortfolio;

/* Temporary source of profile data until a Profile entity/CRUD is introduced
 * (see AI_IMPLEMENTATION_GUIDE.md backlog). Bound from the "PortfolioProfile"
 * configuration section so no personal data is hard-coded in application code. */
public class PortfolioProfileOptions
{
    public string? DisplayName { get; set; }
    public string? Headline { get; set; }
    public string? Bio { get; set; }
    public string? AvatarUrl { get; set; }
    public string? CvUrl { get; set; }
    public string? Email { get; set; }
    public List<string> SocialLinks { get; set; } = new();
}
