using System;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;

namespace Portfolio3D.Profiles;

/* Ensures a database always has exactly one Profile row. Runs in every
 * environment (this is the site owner's real identity, not demo content),
 * but only ever inserts when no profile exists yet - an admin-edited or
 * previously seeded profile is never overwritten. */
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
                "Vi Quốc Thuận",
                "Software Engineer",
                "Software engineer focused on building modern, maintainable web applications with .NET, Angular and ABP Framework."
            )
        );
    }
}
