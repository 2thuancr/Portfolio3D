using System;
using Shouldly;
using Xunit;

namespace Portfolio3D.Profiles;

public class ProfileTests
{
    private static Profile CreateValidProfile()
    {
        return new Profile(
            Guid.NewGuid(),
            "Jane Doe",
            "Software Engineer",
            "Building things on the web."
        );
    }

    [Fact]
    public void Should_Create_A_Valid_Profile()
    {
        var profile = CreateValidProfile();

        profile.DisplayName.ShouldBe("Jane Doe");
        profile.Headline.ShouldBe("Software Engineer");
        profile.Bio.ShouldBe("Building things on the web.");
        profile.AvatarUrl.ShouldBeNull();
        profile.CvUrl.ShouldBeNull();
        profile.Email.ShouldBeNull();
        profile.SocialLinksJson.ShouldBeNull();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Should_Not_Allow_Invalid_DisplayName(string? invalidDisplayName)
    {
        Should.Throw<ArgumentException>(() =>
            new Profile(Guid.NewGuid(), invalidDisplayName!, "Headline", "Bio")
        );
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Should_Not_Allow_Invalid_Headline(string? invalidHeadline)
    {
        Should.Throw<ArgumentException>(() =>
            new Profile(Guid.NewGuid(), "Jane Doe", invalidHeadline!, "Bio")
        );
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Should_Not_Allow_Invalid_Bio(string? invalidBio)
    {
        Should.Throw<ArgumentException>(() =>
            new Profile(Guid.NewGuid(), "Jane Doe", "Headline", invalidBio!)
        );
    }

    [Fact]
    public void Should_Update_Optional_Fields()
    {
        var profile = CreateValidProfile();

        profile.SetAvatarUrl("https://example.com/avatar.png");
        profile.SetCvUrl("https://example.com/cv.pdf");
        profile.SetEmail("jane@example.com");
        profile.SetSocialLinksJson("{\"gitHubUrl\":\"https://github.com/jane\"}");

        profile.AvatarUrl.ShouldBe("https://example.com/avatar.png");
        profile.CvUrl.ShouldBe("https://example.com/cv.pdf");
        profile.Email.ShouldBe("jane@example.com");
        profile.SocialLinksJson.ShouldBe("{\"gitHubUrl\":\"https://github.com/jane\"}");
    }
}
