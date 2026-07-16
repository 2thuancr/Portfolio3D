using System;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;

namespace Portfolio3D.Profiles;

/* Ensures a fresh database always has exactly one Profile row for the
 * admin to edit, instead of shipping fabricated personal data. Values are
 * generic placeholders, not real identity information. */
public class ProfileDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    private readonly IRepository<Profile, Guid> _profileRepository;
    private readonly IGuidGenerator _guidGenerator;

    public ProfileDataSeedContributor(
        IRepository<Profile, Guid> profileRepository,
        IGuidGenerator guidGenerator)
    {
        _profileRepository = profileRepository;
        _guidGenerator = guidGenerator;
    }

    public virtual async Task SeedAsync(DataSeedContext context)
    {
        if (await _profileRepository.GetCountAsync() > 0)
        {
            return;
        }

        await _profileRepository.InsertAsync(
            new Profile(
                _guidGenerator.Create(),
                "Your Name",
                "Software Engineer",
                "Write a short bio to introduce yourself."
            )
        );
    }
}
