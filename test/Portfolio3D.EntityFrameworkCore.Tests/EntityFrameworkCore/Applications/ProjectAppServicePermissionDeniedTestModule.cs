using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Volo.Abp.Authorization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Modularity;

namespace Portfolio3D.EntityFrameworkCore.Applications;

/* Reverts the AlwaysAllowAuthorization stub (added by Portfolio3DTestBaseModule for
 * convenience in other tests) back to the real authorization pipeline, so that
 * permission enforcement on IProjectAppService can be verified end-to-end. */
[DependsOn(typeof(Portfolio3DEntityFrameworkCoreTestModule))]
public class ProjectAppServicePermissionDeniedTestModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.Replace(ServiceDescriptor.Transient<IAuthorizationService, AbpAuthorizationService>());
        context.Services.Replace(ServiceDescriptor.Transient<IAbpAuthorizationService, AbpAuthorizationService>());
        context.Services.Replace(ServiceDescriptor.Transient<IAbpAuthorizationPolicyProvider, AbpAuthorizationPolicyProvider>());
        context.Services.Replace(ServiceDescriptor.Transient<IMethodInvocationAuthorizationService, MethodInvocationAuthorizationService>());
        context.Services.Replace(ServiceDescriptor.Transient<IPermissionChecker, PermissionChecker>());
    }
}
