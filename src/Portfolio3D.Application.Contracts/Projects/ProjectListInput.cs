using Volo.Abp.Application.Dtos;

namespace Portfolio3D.Projects;

public class ProjectListInput : PagedAndSortedResultRequestDto
{
    public string? Filter { get; set; }
    public bool? IsPublished { get; set; }
    public bool? IsFeatured { get; set; }
}
