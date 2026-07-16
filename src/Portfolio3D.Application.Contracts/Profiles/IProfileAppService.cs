using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Portfolio3D.Profiles;

public interface IProfileAppService : IApplicationService
{
    Task<ProfileDto> GetAsync();

    Task<ProfileDto> UpdateAsync(UpdateProfileDto input);
}
