using System.Net;
using DotOPDS.Dto;
using DotOPDS.Shared.Options;
using FluentAssertions;
using Xunit;

namespace DotOPDS.Tests;

public class OpdsControllerTest : Fixtures.IntegrationTest
{
    private readonly PresentationOptions _presentationOptions;

    public OpdsControllerTest(ApiWebApplicationFactory fixture)
        : base(fixture)
    {
        _presentationOptions = new();
        _factory.Configuration.GetSection(PresentationOptions.ConfigurationKey).Bind(_presentationOptions);
    }

    [Fact]
    public async Task Get_Index_Html()
    {
        var response = await _client.GetAsync("/");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType!.MediaType.Should().Be("text/html");
    }

    [Fact]
    public async Task Get_Index()
    {
        var feed = await _client.GetAndDeserialize<Feed>("/opds");

        feed.Total.Should().Be(1);
        feed.ItemsPerPage.Should().Be(_presentationOptions.PageSize);
        feed.Entries.Should().HaveCount(1);
    }

    [Fact]
    public async Task Get_Search_Index()
    {
        var feed = await _client.GetAndDeserialize<Feed>("/opds/search?q=Андрощук");

        feed.Total.Should().Be(5);
        feed.ItemsPerPage.Should().Be(_presentationOptions.PageSize);
        feed.Entries.Should().HaveCount(5);
    }

    [Fact]
    public async Task Get_Search_Everywhere()
    {
        var feed = await _client.GetAndDeserialize<Feed>("/opds/search/everywhere?q=Андрощук");

        feed.Total.Should().Be(20);
        feed.ItemsPerPage.Should().Be(_presentationOptions.PageSize);
        feed.Entries.Should().HaveCount(10);

        feed = await _client.GetAndDeserialize<Feed>("/opds/search/everywhere?q=Андрощук&page=2");

        feed.Total.Should().Be(20);
        feed.ItemsPerPage.Should().Be(_presentationOptions.PageSize);
        feed.Entries.Should().HaveCount(10);
    }

    [Fact]
    public async Task Get_Search_Advanced()
    {
        var feed = await _client.GetAndDeserialize<Feed>("/opds/search/advanced?q=title:жизнь AND author:александр");

        feed.Total.Should().Be(2);
        feed.ItemsPerPage.Should().Be(_presentationOptions.PageSize);
        feed.Entries.Should().HaveCount(2);

        feed.Entries.Where(x =>
            x.Authors.Where(a => a.Name!.Contains("Коцюбинский")
        ).Any()).Should().HaveCount(2);
    }

    [Fact]
    public async Task Get_Search_ByField()
    {
        var feed = await _client.GetAndDeserialize<Feed>("/opds/search/byField?field=series&q=Как приручить дракона");

        feed.Total.Should().Be(5);
        feed.ItemsPerPage.Should().Be(_presentationOptions.PageSize);
        feed.Entries.Should().HaveCount(5);
    }

    [Fact]
    public async Task Get_RootGenres()
    {
        var feed = await _client.GetAndDeserialize<Feed>("/opds/genres");

        feed.Total.Should().Be(16);
        feed.ItemsPerPage.Should().Be(16);
        feed.Entries.Should().HaveCount(16);
    }

    [Fact]
    public async Task Get_SubGenres()
    {
        var feed = await _client.GetAndDeserialize<Feed>("/opds/genres/Фантастика");

        feed.Total.Should().Be(13);
        feed.ItemsPerPage.Should().Be(13);
        feed.Entries.Should().HaveCount(13);
    }

    [Fact]
    public async Task Get_ByGenre()
    {
        var feed = await _client.GetAndDeserialize<Feed>("/opds/genre/Фантастика~~~Ужасы");

        feed.Total.Should().Be(9);
        feed.ItemsPerPage.Should().Be(_presentationOptions.PageSize);
        feed.Entries.Should().HaveCount(9);
    }

    [Fact]
    public async Task Get_Author()
    {
        var feed = await _client.GetAndDeserialize<Feed>("/opds/author/Иван Кузьмич Андрощук");

        feed.Total.Should().Be(20);
        feed.ItemsPerPage.Should().Be(_presentationOptions.PageSize);
        feed.Entries.Should().HaveCount(10);
    }

    [Fact]
    public async Task Get_Series()
    {
        var feed = await _client.GetAndDeserialize<Feed>("/opds/series/Red Ned Tudor Mysteries");

        feed.Total.Should().Be(6);
        feed.ItemsPerPage.Should().Be(_presentationOptions.PageSize);
        feed.Entries.Should().HaveCount(6);
    }
}
