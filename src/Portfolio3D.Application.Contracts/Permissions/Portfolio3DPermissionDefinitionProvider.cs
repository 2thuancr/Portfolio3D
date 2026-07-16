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

        var projectsPermission = myGroup.AddPermission(
            Portfolio3DPermissions.Projects.Default,
            L("Permission:Projects")
        );
        projectsPermission.AddChild(Portfolio3DPermissions.Projects.Create, L("Permission:Projects.Create"));
        projectsPermission.AddChild(Portfolio3DPermissions.Projects.Update, L("Permission:Projects.Update"));
        projectsPermission.AddChild(Portfolio3DPermissions.Projects.Delete, L("Permission:Projects.Delete"));
        projectsPermission.AddChild(Portfolio3DPermissions.Projects.Publish, L("Permission:Projects.Publish"));
        projectsPermission.AddChild(Portfolio3DPermissions.Projects.Reorder, L("Permission:Projects.Reorder"));

        var skillsPermission = myGroup.AddPermission(
            Portfolio3DPermissions.Skills.Default,
            L("Permission:Skills")
        );
        skillsPermission.AddChild(Portfolio3DPermissions.Skills.Create, L("Permission:Skills.Create"));
        skillsPermission.AddChild(Portfolio3DPermissions.Skills.Update, L("Permission:Skills.Update"));
        skillsPermission.AddChild(Portfolio3DPermissions.Skills.Delete, L("Permission:Skills.Delete"));
        skillsPermission.AddChild(Portfolio3DPermissions.Skills.Publish, L("Permission:Skills.Publish"));
        skillsPermission.AddChild(Portfolio3DPermissions.Skills.Reorder, L("Permission:Skills.Reorder"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<Portfolio3DResource>(name);
    }
}
