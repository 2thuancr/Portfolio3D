using System;
using System.Threading.Tasks;
using Portfolio3D.Data;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;

namespace Portfolio3D.Projects;

/* Seeds real (not fabricated) portfolio project content for local
 * development/testing so the public portfolio and later 3D scene have
 * something to render. Skipped in Production - deploying real project
 * content there is a deliberate admin action, not an implicit code seed.
 * Idempotent by Slug: never duplicates and never overwrites an
 * admin-edited project. */
public class ProjectDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    private readonly IRepository<Project, Guid> _projectRepository;
    private readonly IGuidGenerator _guidGenerator;

    public ProjectDataSeedContributor(
        IRepository<Project, Guid> projectRepository,
        IGuidGenerator guidGenerator)
    {
        _projectRepository = projectRepository;
        _guidGenerator = guidGenerator;
    }

    public virtual async Task SeedAsync(DataSeedContext context)
    {
        if (SeedEnvironmentChecker.IsProductionEnvironment())
        {
            return;
        }

        await SeedProjectAsync(
            slug: "vievent",
            name: "VIEvent",
            summary: "Event management platform for organizing and tracking event registrations.",
            description: "VIEvent is a web application for creating events, managing registrations " +
                          "and tracking attendance, built with ASP.NET Core and Angular.",
            /* TODO: replace with a real hosted thumbnail once one is available;
             * this path assumes a matching asset will be added under
             * angular/src/assets/images/projects/. */
            thumbnailUrl: "/assets/images/projects/vievent-thumbnail.png",
            isFeatured: true,
            displayOrder: 0
        );
    }

    private async Task SeedProjectAsync(
        string slug,
        string name,
        string summary,
        string description,
        string thumbnailUrl,
        bool isFeatured,
        int displayOrder)
    {
        var existing = await _projectRepository.GetListAsync(p => p.Slug == slug);
        if (existing.Count > 0)
        {
            return;
        }

        var project = new Project(
            _guidGenerator.Create(),
            name,
            slug,
            summary,
            thumbnailUrl,
            description,
            demoUrl: null,
            repositoryUrl: null,
            displayOrder: displayOrder
        );

        project.SetFeatured(isFeatured);
        project.Publish();

        await _projectRepository.InsertAsync(project);
    }
}
