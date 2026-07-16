using Portfolio3D.Localization;
using Volo.Abp.Application.Services;

namespace Portfolio3D;

/* Inherit your application services from this class.
 */
public abstract class Portfolio3DAppService : ApplicationService
{
    protected Portfolio3DAppService()
    {
        LocalizationResource = typeof(Portfolio3DResource);
    }
}
