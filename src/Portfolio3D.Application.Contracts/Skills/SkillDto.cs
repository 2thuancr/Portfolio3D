using System;
using Volo.Abp.Application.Dtos;

namespace Portfolio3D.Skills;

public class SkillDto : EntityDto<Guid>
{
    public string Name { get; set; } = null!;
    public string Category { get; set; } = null!;
    public string? IconUrl { get; set; }
    public string? LevelLabel { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsPublished { get; set; }
    public DateTime CreationTime { get; set; }
    public DateTime? LastModificationTime { get; set; }
}
