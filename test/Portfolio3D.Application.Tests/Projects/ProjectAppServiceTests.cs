using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Shouldly;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Modularity;
using Xunit;

namespace Portfolio3D.Projects;

public abstract class ProjectAppServiceTests<TStartupModule> : Portfolio3DApplicationTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    private readonly IProjectAppService _projectAppService;
    private readonly IRepository<Project, Guid> _projectRepository;
    private readonly IDataFilter _dataFilter;

    protected ProjectAppServiceTests()
    {
        _projectAppService = GetRequiredService<IProjectAppService>();
        _projectRepository = GetRequiredService<IRepository<Project, Guid>>();
        _dataFilter = GetRequiredService<IDataFilter>();
    }

    private static CreateProjectDto CreateValidInput(string slug = "vievent", int displayOrder = 0)
    {
        return new CreateProjectDto
        {
            Name = "Vievent",
            Slug = slug,
            Summary = "An event management platform.",
            ThumbnailUrl = "https://example.com/thumbnail.png",
            DisplayOrder = displayOrder
        };
    }

    [Fact]
    public async Task Should_Create_A_Project()
    {
        var dto = await _projectAppService.CreateAsync(CreateValidInput());

        dto.Id.ShouldNotBe(Guid.Empty);
        dto.Slug.ShouldBe("vievent");
        dto.IsPublished.ShouldBeFalse();
    }

    [Fact]
    public async Task Should_Reject_Create_With_Duplicate_Slug()
    {
        await _projectAppService.CreateAsync(CreateValidInput("duplicate-slug"));

        await Should.ThrowAsync<BusinessException>(
            () => _projectAppService.CreateAsync(CreateValidInput("duplicate-slug"))
        );
    }

    [Fact]
    public async Task Should_Update_A_Project()
    {
        var created = await _projectAppService.CreateAsync(CreateValidInput("update-target"));

        var updated = await _projectAppService.UpdateAsync(created.Id, new UpdateProjectDto
        {
            Name = "Vievent v2",
            Slug = "update-target",
            Summary = "Updated summary.",
            ThumbnailUrl = "https://example.com/thumbnail-v2.png",
            DisplayOrder = 5,
            IsFeatured = true
        });

        updated.Name.ShouldBe("Vievent v2");
        updated.DisplayOrder.ShouldBe(5);
        updated.IsFeatured.ShouldBeTrue();
    }

    [Fact]
    public async Task Should_Reject_Update_With_Duplicate_Slug()
    {
        await _projectAppService.CreateAsync(CreateValidInput("first-slug"));
        var second = await _projectAppService.CreateAsync(CreateValidInput("second-slug"));

        await Should.ThrowAsync<BusinessException>(
            () => _projectAppService.UpdateAsync(second.Id, new UpdateProjectDto
            {
                Name = second.Name,
                Slug = "first-slug",
                Summary = second.Summary,
                ThumbnailUrl = second.ThumbnailUrl,
                DisplayOrder = second.DisplayOrder,
                IsFeatured = second.IsFeatured
            })
        );
    }

    [Fact]
    public async Task Should_Filter_List_By_Keyword()
    {
        await _projectAppService.CreateAsync(new CreateProjectDto
        {
            Name = "Portfolio Website",
            Slug = "portfolio-website",
            Summary = "A summary.",
            ThumbnailUrl = "https://example.com/thumb.png"
        });
        await _projectAppService.CreateAsync(new CreateProjectDto
        {
            Name = "Chat Application",
            Slug = "chat-application",
            Summary = "A summary.",
            ThumbnailUrl = "https://example.com/thumb.png"
        });

        var result = await _projectAppService.GetListAsync(new ProjectListInput
        {
            Filter = "Portfolio"
        });

        result.Items.ShouldContain(x => x.Slug == "portfolio-website");
        result.Items.ShouldNotContain(x => x.Slug == "chat-application");
    }

    [Fact]
    public async Task Should_Filter_List_By_IsPublished()
    {
        var published = await _projectAppService.CreateAsync(CreateValidInput("published-project"));
        await _projectAppService.CreateAsync(CreateValidInput("unpublished-project"));

        await _projectAppService.PublishAsync(published.Id);

        var result = await _projectAppService.GetListAsync(new ProjectListInput
        {
            IsPublished = true
        });

        result.Items.ShouldContain(x => x.Slug == "published-project");
        result.Items.ShouldNotContain(x => x.Slug == "unpublished-project");
    }

    [Fact]
    public async Task Should_Publish_And_Unpublish_A_Project()
    {
        var created = await _projectAppService.CreateAsync(CreateValidInput("publish-cycle"));

        await _projectAppService.PublishAsync(created.Id);
        (await _projectAppService.GetAsync(created.Id)).IsPublished.ShouldBeTrue();

        await _projectAppService.UnpublishAsync(created.Id);
        (await _projectAppService.GetAsync(created.Id)).IsPublished.ShouldBeFalse();
    }

    [Fact]
    public async Task Should_SoftDelete_A_Project()
    {
        var created = await _projectAppService.CreateAsync(CreateValidInput("to-be-deleted"));

        await _projectAppService.DeleteAsync(created.Id);

        await Should.ThrowAsync<EntityNotFoundException>(() => _projectAppService.GetAsync(created.Id));

        await WithUnitOfWorkAsync(async () =>
        {
            using (_dataFilter.Disable<ISoftDelete>())
            {
                var queryable = await _projectRepository.GetQueryableAsync();
                queryable.Any(p => p.Id == created.Id).ShouldBeTrue();
            }
        });
    }

    [Fact]
    public async Task Should_Reorder_Projects()
    {
        var first = await _projectAppService.CreateAsync(CreateValidInput("reorder-first", 0));
        var second = await _projectAppService.CreateAsync(CreateValidInput("reorder-second", 1));

        await _projectAppService.ReorderAsync(new ReorderProjectsDto
        {
            Items = new List<ReorderProjectItemDto>
            {
                new() { Id = first.Id, DisplayOrder = 10 },
                new() { Id = second.Id, DisplayOrder = 20 }
            }
        });

        (await _projectAppService.GetAsync(first.Id)).DisplayOrder.ShouldBe(10);
        (await _projectAppService.GetAsync(second.Id)).DisplayOrder.ShouldBe(20);
    }

    [Fact]
    public async Task Should_Reject_Reorder_When_Project_Does_Not_Exist()
    {
        var existing = await _projectAppService.CreateAsync(CreateValidInput("reorder-missing"));

        await Should.ThrowAsync<BusinessException>(
            () => _projectAppService.ReorderAsync(new ReorderProjectsDto
            {
                Items = new List<ReorderProjectItemDto>
                {
                    new() { Id = existing.Id, DisplayOrder = 1 },
                    new() { Id = Guid.NewGuid(), DisplayOrder = 2 }
                }
            })
        );

        (await _projectAppService.GetAsync(existing.Id)).DisplayOrder.ShouldBe(0);
    }

    [Fact]
    public async Task Should_Reject_Reorder_With_Duplicate_Id()
    {
        var existing = await _projectAppService.CreateAsync(CreateValidInput("reorder-dup-id"));

        await Should.ThrowAsync<BusinessException>(
            () => _projectAppService.ReorderAsync(new ReorderProjectsDto
            {
                Items = new List<ReorderProjectItemDto>
                {
                    new() { Id = existing.Id, DisplayOrder = 1 },
                    new() { Id = existing.Id, DisplayOrder = 2 }
                }
            })
        );
    }

    [Fact]
    public async Task Should_Reject_Reorder_With_Duplicate_DisplayOrder()
    {
        var first = await _projectAppService.CreateAsync(CreateValidInput("reorder-dup-order-1"));
        var second = await _projectAppService.CreateAsync(CreateValidInput("reorder-dup-order-2"));

        await Should.ThrowAsync<BusinessException>(
            () => _projectAppService.ReorderAsync(new ReorderProjectsDto
            {
                Items = new List<ReorderProjectItemDto>
                {
                    new() { Id = first.Id, DisplayOrder = 5 },
                    new() { Id = second.Id, DisplayOrder = 5 }
                }
            })
        );
    }
}
