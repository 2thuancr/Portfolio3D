using System;

namespace Portfolio3D.Data;

/* Reads the OS process environment directly (not IConfiguration/IHostEnvironment)
 * so the check works identically across every host that can run a data seeder:
 * DbMigrator (generic host, DOTNET_ENVIRONMENT), HttpApi.Host (web host,
 * ASPNETCORE_ENVIRONMENT) and the integration test host (neither is
 * registered in DI, and no environment variable is set there - which is
 * intentionally treated as safe-to-seed, matching "Development/test có thể
 * seed dữ liệu mẫu"). Only an explicit "Production" value opts out. */
public static class SeedEnvironmentChecker
{
    public static bool IsProductionEnvironment()
    {
        var environmentName =
            Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ??
            Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        return string.Equals(environmentName, "Production", StringComparison.OrdinalIgnoreCase);
    }
}
