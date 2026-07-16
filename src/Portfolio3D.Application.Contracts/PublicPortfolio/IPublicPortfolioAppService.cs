using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Portfolio3D.PublicPortfolio;

public interface IPublicPortfolioAppService : IApplicationService
{
    Task<PublicPortfolioDto> GetAsync();
}
