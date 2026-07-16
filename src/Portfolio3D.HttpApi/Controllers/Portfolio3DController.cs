using Portfolio3D.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace Portfolio3D.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class Portfolio3DController : AbpControllerBase
{
    protected Portfolio3DController()
    {
        LocalizationResource = typeof(Portfolio3DResource);
    }
}
