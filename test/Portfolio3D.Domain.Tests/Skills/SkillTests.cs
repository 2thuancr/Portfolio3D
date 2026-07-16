using System;
using Shouldly;
using Xunit;

namespace Portfolio3D.Skills;

public class SkillTests
{
    private static Skill CreateValidSkill()
    {
        return new Skill(
            Guid.NewGuid(),
            ".NET",
            "Backend"
        );
    }

    [Fact]
    public void Should_Publish_A_Skill()
    {
        var skill = CreateValidSkill();
        skill.IsPublished.ShouldBeFalse();

        skill.Publish();

        skill.IsPublished.ShouldBeTrue();
    }

    [Fact]
    public void Should_Unpublish_A_Skill()
    {
        var skill = CreateValidSkill();
        skill.Publish();

        skill.Unpublish();

        skill.IsPublished.ShouldBeFalse();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Should_Not_Allow_Invalid_Name(string? invalidName)
    {
        Should.Throw<ArgumentException>(() =>
            new Skill(Guid.NewGuid(), invalidName!, "Backend")
        );
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Should_Not_Allow_Invalid_Category(string? invalidCategory)
    {
        Should.Throw<ArgumentException>(() =>
            new Skill(Guid.NewGuid(), ".NET", invalidCategory!)
        );
    }

    [Fact]
    public void Should_Not_Allow_Negative_DisplayOrder()
    {
        var skill = CreateValidSkill();

        Should.Throw<ArgumentOutOfRangeException>(() => skill.SetDisplayOrder(-1));
    }

    [Fact]
    public void Should_Allow_Null_IconUrl_And_LevelLabel()
    {
        var skill = CreateValidSkill();

        skill.IconUrl.ShouldBeNull();
        skill.LevelLabel.ShouldBeNull();
    }
}
