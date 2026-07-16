using System;
using System.ComponentModel.DataAnnotations;

namespace Portfolio3D.Projects;

public class ReorderProjectItemDto
{
    [Required]
    public Guid Id { get; set; }

    [Range(0, int.MaxValue)]
    public int DisplayOrder { get; set; }
}
