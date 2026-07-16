using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Portfolio3D.Projects;

public class Project : FullAuditedAggregateRoot<Guid>
{
    public string Name { get; private set; } = null!;
    public string Slug { get; private set; } = null!;
    public string Summary { get; private set; } = null!;
    public string? Description { get; private set; }
    public string ThumbnailUrl { get; private set; } = null!;
    public string? DemoUrl { get; private set; }
    public string? RepositoryUrl { get; private set; }
    public bool IsPublished { get; private set; }
    public bool IsFeatured { get; private set; }
    public int DisplayOrder { get; private set; }

    protected Project()
    {
    }

    public Project(
        Guid id,
        string name,
        string slug,
        string summary,
        string thumbnailUrl,
        string? description = null,
        string? demoUrl = null,
        string? repositoryUrl = null,
        int displayOrder = 0)
        : base(id)
    {
        SetName(name);
        SetSlug(slug);
        SetSummary(summary);
        SetThumbnailUrl(thumbnailUrl);
        SetDescription(description);
        SetDemoUrl(demoUrl);
        SetRepositoryUrl(repositoryUrl);
        SetDisplayOrder(displayOrder);
        IsPublished = false;
        IsFeatured = false;
    }

    public void SetName(string name)
    {
        Name = Check.NotNullOrWhiteSpace(name, nameof(name), ProjectConsts.MaxNameLength);
    }

    public void SetSlug(string slug)
    {
        Check.NotNullOrWhiteSpace(slug, nameof(slug), ProjectConsts.MaxSlugLength);
        Slug = slug.Trim().ToLowerInvariant();
    }

    public void SetSummary(string summary)
    {
        Summary = Check.NotNullOrWhiteSpace(summary, nameof(summary), ProjectConsts.MaxSummaryLength);
    }

    public void SetThumbnailUrl(string thumbnailUrl)
    {
        ThumbnailUrl = Check.NotNullOrWhiteSpace(thumbnailUrl, nameof(thumbnailUrl), ProjectConsts.MaxUrlLength);
    }

    public void SetDescription(string? description)
    {
        Description = description;
    }

    public void SetDemoUrl(string? demoUrl)
    {
        DemoUrl = demoUrl;
    }

    public void SetRepositoryUrl(string? repositoryUrl)
    {
        RepositoryUrl = repositoryUrl;
    }

    public void SetFeatured(bool isFeatured)
    {
        IsFeatured = isFeatured;
    }

    public void SetDisplayOrder(int displayOrder)
    {
        if (displayOrder < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(displayOrder), "DisplayOrder must not be negative.");
        }

        DisplayOrder = displayOrder;
    }

    public void Publish()
    {
        IsPublished = true;
    }

    public void Unpublish()
    {
        IsPublished = false;
    }
}
