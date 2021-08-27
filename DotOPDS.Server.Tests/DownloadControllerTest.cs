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
        var file = await _client.GetAsync("/download/file/24ce7b01-ca08-4c7a-99b5-594e967eb8b0/fb2");

        file.Content.Headers.ContentType?.MediaType.Should().Be("application/fb2");
        file.Content.Headers.ContentDisposition!.FileNameStar.Should().Be("Владимир Казимирович Венгловский - Хардкор.fb2");

        var data = await file.Content.ReadAsStreamAsync();
        data.Length.Should().Be(1820);
    }

    [Fact]
    public async Task Get_Fb2_Converted_Book()
    {
        var file = await _client.GetAsync("/download/file/24ce7b01-ca08-4c7a-99b5-594e967eb8b0/epub");

        file.Content.Headers.ContentType?.MediaType.Should().Be("application/epub+zip");
        file.Content.Headers.ContentDisposition!.FileNameStar.Should().Be("Владимир Казимирович Венгловский - Хардкор.epub");

        var data = await file.Content.ReadAsStreamAsync();
        data.Length.Should().Be(1820);
    }
}
