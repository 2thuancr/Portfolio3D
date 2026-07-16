using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Portfolio3D.Skills;

public class ReorderSkillsDto
{
    [Required]
    public List<ReorderSkillItemDto> Items { get; set; } = new();
}
