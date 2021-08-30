using FluentAssertions;
using Xunit;

namespace DotOPDS.Tests;

public class DownloadControllerTest : Fixtures.IntegrationTest
{
    public DownloadControllerTest(ApiWebApplicationFactory fixture)
        : base(fixture) { }

    [Fact]
    public async Task Get_Fb2_Book()
    {
        var file = await _client.GetAsync("/download/file/798a5d93-e5e5-48c0-9af6-b1e76ee20ee1/fb2");

        file.Content.Headers.ContentType?.MediaType.Should().Be("application/fb2");
        file.Content.Headers.ContentDisposition!.FileNameStar.Should().Be("Владимир Казимирович Венгловский - Хардкор.fb2");

        var data = await file.Content.ReadAsStreamAsync();
        data.Length.Should().Be(1820);
    }

    [Fact(Skip = "Find a better way to launch converter cross-platform")]
    public async Task Get_Fb2_Converted_Book()
    {
        var file = await _client.GetAsync("/download/file/798a5d93-e5e5-48c0-9af6-b1e76ee20ee1/epub");

        file.Content.Headers.ContentType?.MediaType.Should().Be("application/epub+zip");
        file.Content.Headers.ContentDisposition!.FileNameStar.Should().Be("Владимир Казимирович Венгловский - Хардкор.epub");

        var data = await file.Content.ReadAsStreamAsync();
        data.Length.Should().Be(1820);
    }
}
