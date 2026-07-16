using System.ComponentModel.DataAnnotations;

namespace Portfolio3D.Profiles;

public class UpdateProfileDto
{
    [Required]
    [StringLength(ProfileConsts.MaxDisplayNameLength)]
    public string DisplayName { get; set; } = null!;

    [Required]
    [StringLength(ProfileConsts.MaxHeadlineLength)]
    public string Headline { get; set; } = null!;

    [Required]
    [StringLength(ProfileConsts.MaxBioLength)]
    public string Bio { get; set; } = null!;

    [StringLength(ProfileConsts.MaxUrlLength)]
    [Url]
    public string? AvatarUrl { get; set; }

    [StringLength(ProfileConsts.MaxUrlLength)]
    [Url]
    public string? CvUrl { get; set; }

    [StringLength(ProfileConsts.MaxEmailLength)]
    [EmailAddress]
    public string? Email { get; set; }

    public SocialLinksDto SocialLinks { get; set; } = new();
}
