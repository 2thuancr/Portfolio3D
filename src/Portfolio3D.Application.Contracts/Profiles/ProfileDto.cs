using System;
using Volo.Abp.Application.Dtos;

namespace Portfolio3D.Profiles;

public class ProfileDto : EntityDto<Guid>
{
    public string DisplayName { get; set; } = null!;
    public string Headline { get; set; } = null!;
    public string Bio { get; set; } = null!;
    public string? AvatarUrl { get; set; }
    public string? CvUrl { get; set; }
    public string? Email { get; set; }
    public SocialLinksDto SocialLinks { get; set; } = new();
    public DateTime CreationTime { get; set; }
    public DateTime? LastModificationTime { get; set; }
}
