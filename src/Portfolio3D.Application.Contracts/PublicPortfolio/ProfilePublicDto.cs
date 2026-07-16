using System.Collections.Generic;

namespace Portfolio3D.PublicPortfolio;

public class ProfilePublicDto
{
    public string DisplayName { get; set; } = null!;
    public string? Headline { get; set; }
    public string? Bio { get; set; }
    public string? AvatarUrl { get; set; }
    public string? CvUrl { get; set; }
    public string? Email { get; set; }
    public List<string> SocialLinks { get; set; } = new();
}
