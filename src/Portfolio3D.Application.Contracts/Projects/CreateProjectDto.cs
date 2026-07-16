using System.ComponentModel.DataAnnotations;

namespace Portfolio3D.Projects;

public class CreateProjectDto
{
    [Required]
    [StringLength(ProjectConsts.MaxNameLength)]
    public string Name { get; set; } = null!;

    [Required]
    [StringLength(ProjectConsts.MaxSlugLength)]
    public string Slug { get; set; } = null!;

    [Required]
    [StringLength(ProjectConsts.MaxSummaryLength)]
    public string Summary { get; set; } = null!;

    [StringLength(ProjectConsts.MaxDescriptionLength)]
    public string? Description { get; set; }

    [Required]
    [StringLength(ProjectConsts.MaxUrlLength)]
    [Url]
    public string ThumbnailUrl { get; set; } = null!;

    [StringLength(ProjectConsts.MaxUrlLength)]
    [Url]
    public string? DemoUrl { get; set; }

    [StringLength(ProjectConsts.MaxUrlLength)]
    [Url]
    public string? RepositoryUrl { get; set; }

    public bool IsFeatured { get; set; }

    [Range(0, int.MaxValue)]
    public int DisplayOrder { get; set; }
}
