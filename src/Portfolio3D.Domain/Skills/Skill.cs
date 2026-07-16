using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Portfolio3D.Skills;

public class Skill : FullAuditedAggregateRoot<Guid>
{
    public string Name { get; private set; } = null!;
    public string Category { get; private set; } = null!;
    public string? IconUrl { get; private set; }
    public string? LevelLabel { get; private set; }
    public int DisplayOrder { get; private set; }
    public bool IsPublished { get; private set; }

    protected Skill()
    {
    }

    public Skill(
        Guid id,
        string name,
        string category,
        string? iconUrl = null,
        string? levelLabel = null,
        int displayOrder = 0)
        : base(id)
    {
        SetName(name);
        SetCategory(category);
        SetIconUrl(iconUrl);
        SetLevelLabel(levelLabel);
        SetDisplayOrder(displayOrder);
        IsPublished = false;
    }

    public void SetName(string name)
    {
        Name = Check.NotNullOrWhiteSpace(name, nameof(name), SkillConsts.MaxNameLength);
    }

    public void SetCategory(string category)
    {
        Category = Check.NotNullOrWhiteSpace(category, nameof(category), SkillConsts.MaxCategoryLength);
    }

    public void SetIconUrl(string? iconUrl)
    {
        IconUrl = iconUrl;
    }

    public void SetLevelLabel(string? levelLabel)
    {
        LevelLabel = levelLabel;
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
