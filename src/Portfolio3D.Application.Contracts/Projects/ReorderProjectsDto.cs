using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Portfolio3D.Projects;

public class ReorderProjectsDto
{
    [Required]
    public List<ReorderProjectItemDto> Items { get; set; } = new();
}
