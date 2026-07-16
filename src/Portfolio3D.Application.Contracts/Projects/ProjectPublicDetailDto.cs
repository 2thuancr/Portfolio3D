using System;
using Volo.Abp.Application.Dtos;

namespace Portfolio3D.Projects;

public class ProjectPublicDetailDto : EntityDto<Guid>
{
    public string Name { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public string Summary { get; set; } = null!;
    public string? Description { get; set; }
    public string ThumbnailUrl { get; set; } = null!;
    public string? DemoUrl { get; set; }
    public string? RepositoryUrl { get; set; }
    public bool IsFeatured { get; set; }
}
