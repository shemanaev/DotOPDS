using FluentAssertions;
using DotOPDS.Contract.Models;
using Xunit;

namespace DotOPDS.BookProvider.Inpx.Tests;

public class TestInpxParser
{
    private readonly string _filename = "../../../TestData/demo.inpx";
    private List<Book>? _records;

    public async Task<InpxParser> GetParser() =>
        await InpxParser.Open(_filename, new Genres(new MockTranslator()));

    [Fact]
    public async Task CollectionInfoParsingTest()
    {
        var parser = await GetParser();

        parser.IsFb2.Should().BeTrue();
        parser.Name.Should().Be("Lib.rus.ec Local [FB2]");
        parser.FileName.Should().Be("librusec_local_fb2");
        parser.Description.Should().Be("Архивы библиотеки Lib.rus.ec (fb2)\nhttp://lib.rus.ec/");
        parser.Version.Should().Be("20160301");
    }

    [Fact]
    public async Task EntriesParsingTest()
    {
        var parser = await GetParser();

        _records = await parser.GetBooksAsync().ToListAsync();

        _records.Count.Should().Be(101);

        _records.Where(x => x.Authors.Any(a => a.LastName == "Андрощук")).Count().Should().Be(20);
    }
}
