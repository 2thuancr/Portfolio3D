using System.Linq;
using Shouldly;
using Xunit;

namespace Portfolio3D.Projects;

public class ProjectPublicAppServiceContractTests
{
    [Fact]
    public void Should_Not_Expose_Any_Write_Operation()
    {
        var writeVerbs = new[] { "Create", "Update", "Delete", "Publish", "Unpublish", "Reorder" };

        var methodNames = typeof(IProjectPublicAppService)
            .GetMethods()
            .Select(m => m.Name)
            .ToList();

        methodNames.ShouldNotContain(name => writeVerbs.Any(verb => name.Contains(verb)));
    }
}
