using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Portfolio3D.Permissions;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;

namespace Portfolio3D.Profiles;

public class ProfileAppService : Portfolio3DAppService, IProfileAppService
{
    private readonly IRepository<Profile, Guid> _profileRepository;

    public ProfileAppService(IRepository<Profile, Guid> profileRepository)
    {
        _profileRepository = profileRepository;
    }

    [Authorize(Portfolio3DPermissions.Profile.Default)]
    public virtual async Task<ProfileDto> GetAsync()
    {
        var profile = await GetActiveProfileOrNullAsync();

        if (profile == null)
        {
            throw new EntityNotFoundException(typeof(Profile));
        }

        return MapToDto(profile);
    }

    [Authorize(Portfolio3DPermissions.Profile.Update)]
    public virtual async Task<ProfileDto> UpdateAsync(UpdateProfileDto input)
    {
        var socialLinksJson = SocialLinksJsonSerializer.Serialize(input.SocialLinks);
        var profile = await GetActiveProfileOrNullAsync();

        if (profile == null)
        {
            /* First-time save: there is no active profile yet (should be rare,
             * since ProfileDataSeedContributor seeds one on migration), so this
             * creates the single active Profile row rather than requiring a
             * separate CreateAsync endpoint. */
            profile = new Profile(
                GuidGenerator.Create(),
                input.DisplayName,
                input.Headline,
                input.Bio,
                input.AvatarUrl,
                input.CvUrl,
                input.Email,
                socialLinksJson
            );

            await _profileRepository.InsertAsync(profile);
        }
        else
        {
            profile.SetDisplayName(input.DisplayName);
            profile.SetHeadline(input.Headline);
            profile.SetBio(input.Bio);
            profile.SetAvatarUrl(input.AvatarUrl);
            profile.SetCvUrl(input.CvUrl);
            profile.SetEmail(input.Email);
            profile.SetSocialLinksJson(socialLinksJson);

            await _profileRepository.UpdateAsync(profile);
        }

        return MapToDto(profile);
    }

    private async Task<Profile?> GetActiveProfileOrNullAsync()
    {
        var queryable = await _profileRepository.GetQueryableAsync();
        return await AsyncExecuter.FirstOrDefaultAsync(queryable);
    }

    private static ProfileDto MapToDto(Profile profile)
    {
        return new ProfileDto
        {
            Id = profile.Id,
            DisplayName = profile.DisplayName,
            Headline = profile.Headline,
            Bio = profile.Bio,
            AvatarUrl = profile.AvatarUrl,
            CvUrl = profile.CvUrl,
            Email = profile.Email,
            SocialLinks = SocialLinksJsonSerializer.Deserialize(profile.SocialLinksJson),
            CreationTime = profile.CreationTime,
            LastModificationTime = profile.LastModificationTime
        };
    }
}
