using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Volo.Abp.Authorization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Modularity;

namespace Portfolio3D.EntityFrameworkCore.Applications;

// The shared test module enables AlwaysAllowAuthorization.
// This module restores ABP's real authorization pipeline
// for permission-denied integration tests.
// Reuse this module for future permission-denied tests instead of
// creating a new one per feature.
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
