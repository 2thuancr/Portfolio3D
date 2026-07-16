using System;
using System.ComponentModel.DataAnnotations;

namespace Portfolio3D.Skills;

public class ReorderSkillItemDto
{
    [Required]
    public Guid Id { get; set; }

    [Range(0, int.MaxValue)]
    public int DisplayOrder { get; set; }
}
