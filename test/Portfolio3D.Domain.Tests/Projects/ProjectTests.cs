using System;
using Shouldly;
using Xunit;

namespace Portfolio3D.Projects;

public class ProjectTests
{
    private static Project CreateValidProject()
    {
        return new Project(
            Guid.NewGuid(),
            "Vievent",
            "Vievent",
            "An event management platform.",
            "https://example.com/thumbnail.png"
        );
    }

    [Fact]
    public void Should_Publish_A_Project()
    {
        var project = CreateValidProject();
        project.IsPublished.ShouldBeFalse();

        project.Publish();

        project.IsPublished.ShouldBeTrue();
    }

    [Fact]
    public void Should_Unpublish_A_Project()
    {
        var project = CreateValidProject();
        project.Publish();

        project.Unpublish();

        project.IsPublished.ShouldBeFalse();
    }

    [Fact]
    public void Should_Normalize_Slug_To_Lowercase()
    {
        var project = CreateValidProject();

        project.Slug.ShouldBe("vievent");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Should_Not_Allow_Invalid_Name(string? invalidName)
    {
        Should.Throw<ArgumentException>(() =>
            new Project(
                Guid.NewGuid(),
                invalidName!,
                "vievent",
                "An event management platform.",
                "https://example.com/thumbnail.png"
            )
        );
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Should_Not_Allow_Invalid_Slug(string? invalidSlug)
    {
        Should.Throw<ArgumentException>(() =>
            new Project(
                Guid.NewGuid(),
                "Vievent",
                invalidSlug!,
                "An event management platform.",
                "https://example.com/thumbnail.png"
            )
        );
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Should_Not_Allow_Invalid_Summary(string? invalidSummary)
    {
        Should.Throw<ArgumentException>(() =>
            new Project(
                Guid.NewGuid(),
                "Vievent",
                "vievent",
                invalidSummary!,
                "https://example.com/thumbnail.png"
            )
        );
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Should_Not_Allow_Invalid_ThumbnailUrl(string? invalidThumbnailUrl)
    {
        Should.Throw<ArgumentException>(() =>
            new Project(
                Guid.NewGuid(),
                "Vievent",
                "vievent",
                "An event management platform.",
                invalidThumbnailUrl!
            )
        );
    }

    [Fact]
    public void Should_Not_Allow_Negative_DisplayOrder()
    {
        var project = CreateValidProject();

        Should.Throw<ArgumentOutOfRangeException>(() => project.SetDisplayOrder(-1));
    }
}
