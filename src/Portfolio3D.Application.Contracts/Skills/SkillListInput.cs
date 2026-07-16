using Volo.Abp.Application.Dtos;

namespace Portfolio3D.Skills;

public class SkillListInput : PagedAndSortedResultRequestDto
{
    public string? Filter { get; set; }
    public bool? IsPublished { get; set; }
}
