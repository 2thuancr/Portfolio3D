using System;
using Volo.Abp.Application.Dtos;

namespace Portfolio3D.Projects;

public class ProjectListDto : EntityDto<Guid>
{
    public string Name { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public string Summary { get; set; } = null!;
    public string ThumbnailUrl { get; set; } = null!;
    public bool IsPublished { get; set; }
    public bool IsFeatured { get; set; }
    public int DisplayOrder { get; set; }
}
