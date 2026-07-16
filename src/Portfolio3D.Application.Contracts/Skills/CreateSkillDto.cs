using System.ComponentModel.DataAnnotations;

namespace Portfolio3D.Skills;

public class CreateSkillDto
{
    [Required]
    [StringLength(SkillConsts.MaxNameLength)]
    public string Name { get; set; } = null!;

    [Required]
    [StringLength(SkillConsts.MaxCategoryLength)]
    public string Category { get; set; } = null!;

    [StringLength(SkillConsts.MaxIconUrlLength)]
    [Url]
    public string? IconUrl { get; set; }

    [StringLength(SkillConsts.MaxLevelLabelLength)]
    public string? LevelLabel { get; set; }

    [Range(0, int.MaxValue)]
    public int DisplayOrder { get; set; }
}
