using Microsoft.Extensions.Localization;
using Portfolio3D.Localization;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace Portfolio3D;

[Dependency(ReplaceServices = true)]
public class Portfolio3DBrandingProvider : DefaultBrandingProvider
{
    private IStringLocalizer<Portfolio3DResource> _localizer;

    public Portfolio3DBrandingProvider(IStringLocalizer<Portfolio3DResource> localizer)
    {
        _localizer = localizer;
    }

    public override string AppName => _localizer["AppName"];
}
