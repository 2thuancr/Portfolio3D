using Portfolio3D.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;

namespace Portfolio3D.Permissions;

public class Portfolio3DPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(Portfolio3DPermissions.GroupName);

        //Define your own permissions here. Example:
        //myGroup.AddPermission(Portfolio3DPermissions.MyPermission1, L("Permission:MyPermission1"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<Portfolio3DResource>(name);
    }
}
