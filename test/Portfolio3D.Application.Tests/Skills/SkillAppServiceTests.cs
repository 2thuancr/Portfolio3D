using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Shouldly;
using Volo.Abp;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Modularity;
using Xunit;

namespace Portfolio3D.Skills;

public abstract class SkillAppServiceTests<TStartupModule> : Portfolio3DApplicationTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    private readonly ISkillAppService _skillAppService;

    protected SkillAppServiceTests()
    {
        _skillAppService = GetRequiredService<ISkillAppService>();
    }

    private static CreateSkillDto CreateValidInput(
        string name = ".NET",
        string category = "Backend",
        int displayOrder = 0)
    {
        return new CreateSkillDto
        {
            Name = name,
            Category = category,
            DisplayOrder = displayOrder
        };
    }

    [Fact]
    public async Task Should_Create_A_Skill()
    {
        var dto = await _skillAppService.CreateAsync(CreateValidInput());

        dto.Id.ShouldNotBe(Guid.Empty);
        dto.Name.ShouldBe(".NET");
        dto.Category.ShouldBe("Backend");
        dto.IsPublished.ShouldBeFalse();
    }

    [Fact]
    public async Task Should_Update_A_Skill()
    {
        var created = await _skillAppService.CreateAsync(CreateValidInput());

        var updated = await _skillAppService.UpdateAsync(created.Id, new UpdateSkillDto
        {
            Name = "C#",
            Category = "Language",
            DisplayOrder = 3,
            IconUrl = "https://example.com/icon.png",
            LevelLabel = "Advanced"
        });

        updated.Name.ShouldBe("C#");
        updated.Category.ShouldBe("Language");
        updated.DisplayOrder.ShouldBe(3);
        updated.IconUrl.ShouldBe("https://example.com/icon.png");
        updated.LevelLabel.ShouldBe("Advanced");
    }

    [Fact]
    public async Task Should_SoftDelete_A_Skill()
    {
        var created = await _skillAppService.CreateAsync(CreateValidInput());

        await _skillAppService.DeleteAsync(created.Id);

        await Should.ThrowAsync<EntityNotFoundException>(() => _skillAppService.GetAsync(created.Id));
    }

    [Fact]
    public async Task Should_Publish_And_Unpublish_A_Skill()
    {
        var created = await _skillAppService.CreateAsync(CreateValidInput());

        await _skillAppService.PublishAsync(created.Id);
        (await _skillAppService.GetAsync(created.Id)).IsPublished.ShouldBeTrue();

        await _skillAppService.UnpublishAsync(created.Id);
        (await _skillAppService.GetAsync(created.Id)).IsPublished.ShouldBeFalse();
    }

    [Fact]
    public async Task Should_Filter_List_By_Keyword()
    {
        await _skillAppService.CreateAsync(CreateValidInput("Angular", "Frontend"));
        await _skillAppService.CreateAsync(CreateValidInput("PostgreSQL", "Database"));

        var result = await _skillAppService.GetListAsync(new SkillListInput
        {
            Filter = "Angular"
        });

        result.Items.ShouldContain(x => x.Name == "Angular");
        result.Items.ShouldNotContain(x => x.Name == "PostgreSQL");
    }

    [Fact]
    public async Task Should_Reorder_Skills()
    {
        var first = await _skillAppService.CreateAsync(CreateValidInput("First", "Backend", 0));
        var second = await _skillAppService.CreateAsync(CreateValidInput("Second", "Backend", 1));

        await _skillAppService.ReorderAsync(new ReorderSkillsDto
        {
            Items = new List<ReorderSkillItemDto>
            {
                new() { Id = first.Id, DisplayOrder = 10 },
                new() { Id = second.Id, DisplayOrder = 20 }
            }
        });

        (await _skillAppService.GetAsync(first.Id)).DisplayOrder.ShouldBe(10);
        (await _skillAppService.GetAsync(second.Id)).DisplayOrder.ShouldBe(20);
    }

    [Fact]
    public async Task Should_Reject_Reorder_With_Duplicate_Id()
    {
        var skill = await _skillAppService.CreateAsync(CreateValidInput());

        await Should.ThrowAsync<BusinessException>(
            () => _skillAppService.ReorderAsync(new ReorderSkillsDto
            {
                Items = new List<ReorderSkillItemDto>
                {
                    new() { Id = skill.Id, DisplayOrder = 1 },
                    new() { Id = skill.Id, DisplayOrder = 2 }
                }
            })
        );
    }

    [Fact]
    public async Task Should_Reject_Reorder_With_Duplicate_DisplayOrder()
    {
        var first = await _skillAppService.CreateAsync(CreateValidInput("First"));
        var second = await _skillAppService.CreateAsync(CreateValidInput("Second"));

        await Should.ThrowAsync<BusinessException>(
            () => _skillAppService.ReorderAsync(new ReorderSkillsDto
            {
                Items = new List<ReorderSkillItemDto>
                {
                    new() { Id = first.Id, DisplayOrder = 5 },
                    new() { Id = second.Id, DisplayOrder = 5 }
                }
            })
        );
    }

    [Fact]
    public async Task Should_Reject_Reorder_When_Skill_Does_Not_Exist()
    {
        var existing = await _skillAppService.CreateAsync(CreateValidInput());

        await Should.ThrowAsync<BusinessException>(
            () => _skillAppService.ReorderAsync(new ReorderSkillsDto
            {
                Items = new List<ReorderSkillItemDto>
                {
                    new() { Id = existing.Id, DisplayOrder = 1 },
                    new() { Id = Guid.NewGuid(), DisplayOrder = 2 }
                }
            })
        );
    }
}
