using System.Text.Json;

namespace Portfolio3D.Profiles;

/* Profile.SocialLinksJson stores a single JSON blob (per blueprint field
 * list) instead of a separate table, per Task 008 scope. This helper keeps
 * the (de)serialization logic in one place for ProfileAppService and
 * PublicPortfolioAppService. */
internal static class SocialLinksJsonSerializer
{
    public static string? Serialize(SocialLinksDto? socialLinks)
    {
        if (socialLinks == null)
        {
            return null;
        }

        var isEmpty = socialLinks.GitHubUrl == null &&
                      socialLinks.LinkedInUrl == null &&
                      socialLinks.FacebookUrl == null &&
                      socialLinks.WebsiteUrl == null;

        return isEmpty ? null : JsonSerializer.Serialize(socialLinks);
    }

    public static SocialLinksDto Deserialize(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return new SocialLinksDto();
        }

        return JsonSerializer.Deserialize<SocialLinksDto>(json) ?? new SocialLinksDto();
    }
}
