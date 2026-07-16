using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Portfolio3D.Profiles;

public class Profile : FullAuditedAggregateRoot<Guid>
{
    public string DisplayName { get; private set; } = null!;
    public string Headline { get; private set; } = null!;
    public string Bio { get; private set; } = null!;
    public string? AvatarUrl { get; private set; }
    public string? CvUrl { get; private set; }
    public string? Email { get; private set; }
    public string? SocialLinksJson { get; private set; }

    protected Profile()
    {
    }

    public Profile(
        Guid id,
        string displayName,
        string headline,
        string bio,
        string? avatarUrl = null,
        string? cvUrl = null,
        string? email = null,
        string? socialLinksJson = null)
        : base(id)
    {
        SetDisplayName(displayName);
        SetHeadline(headline);
        SetBio(bio);
        SetAvatarUrl(avatarUrl);
        SetCvUrl(cvUrl);
        SetEmail(email);
        SetSocialLinksJson(socialLinksJson);
    }

    public void SetDisplayName(string displayName)
    {
        DisplayName = Check.NotNullOrWhiteSpace(displayName, nameof(displayName), ProfileConsts.MaxDisplayNameLength);
    }

    public void SetHeadline(string headline)
    {
        Headline = Check.NotNullOrWhiteSpace(headline, nameof(headline), ProfileConsts.MaxHeadlineLength);
    }

    public void SetBio(string bio)
    {
        Bio = Check.NotNullOrWhiteSpace(bio, nameof(bio), ProfileConsts.MaxBioLength);
    }

    public void SetAvatarUrl(string? avatarUrl)
    {
        AvatarUrl = avatarUrl;
    }

    public void SetCvUrl(string? cvUrl)
    {
        CvUrl = cvUrl;
    }

    public void SetEmail(string? email)
    {
        Email = email;
    }

    public void SetSocialLinksJson(string? socialLinksJson)
    {
        SocialLinksJson = socialLinksJson;
    }
}
