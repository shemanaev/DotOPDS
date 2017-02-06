using DotOPDS.Models;
using DotOPDS.Plugin.BookProvider.Inpx;
using FluentAssertions;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DotOPDS.Tests
{
    [TestFixture("Data/demo.inpx")]
    public class TestInpxParser
    {
        private string filename;
        private List<Book> records = new List<Book>();

        public TestInpxParser(string filename)
        {
            this.filename = filename;
        }

        [Test]
        public async Task ParsingFileTest()
        {
            var parser = new InpxParser(Utils.GetPath(filename));

            parser.IsFb2.Should().BeTrue();
            parser.Name.Should().Be("Lib.rus.ec Local [FB2]");
            parser.FileName.Should().Be("librusec_local_fb2");
            parser.Description.Should().Be("Архивы библиотеки Lib.rus.ec (fb2)\nhttp://lib.rus.ec/");
            parser.Version.Should().Be("20160301");

            parser.OnNewEntry += Parser_OnNewEntry;
            parser.OnFinished += Parser_OnFinished;
            await parser.Parse();
        }

        private void Parser_OnFinished(object sender)
        {
            // cmd.exe
            // chcp 65001
            // .\DotOPDS\bin\Debug\DotOPDS.exe fixture .\DotOPDS.Tests\Data\demo.inpx > .\DotOPDS.Tests\InpxDemoFixture.cs
            //records.Count.Should().Be(InpxDemoFixture.Result.Count);
            //records.ShouldBeEquivalentTo(InpxDemoFixture.Result);
        }

        private void Parser_OnNewEntry(object sender, NewEntryEventArgs e)
        {
            records.Add(e.Book);
        }
    }
}
