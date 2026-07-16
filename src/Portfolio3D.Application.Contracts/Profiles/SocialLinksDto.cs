using System.ComponentModel.DataAnnotations;

namespace Portfolio3D.Profiles;

public class SocialLinksDto
{
    [StringLength(ProfileConsts.MaxUrlLength)]
    [Url]
    public string? GitHubUrl { get; set; }

    [StringLength(ProfileConsts.MaxUrlLength)]
    [Url]
    public string? LinkedInUrl { get; set; }

    [StringLength(ProfileConsts.MaxUrlLength)]
    [Url]
    public string? FacebookUrl { get; set; }

    [StringLength(ProfileConsts.MaxUrlLength)]
    [Url]
    public string? WebsiteUrl { get; set; }
}
