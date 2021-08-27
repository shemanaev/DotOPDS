using DotOPDS.Contract;
using DotOPDS.Contract.Models;
using DotOPDS.Contract.Plugins;

namespace DotOPDS.BookProvider.Inpx
{
    public class InpxBookProvider : IBookProvider
    {
        public Version Version => new(1, 0);
        public string Name => "inpx importer";
        public string Command => "inpx";
        public string Help => @"Arguments:
  inpx file                     inpx file to import.
";

        private readonly ITranslator _translator;

        public IEnumerable<IndexField> IndexFields => new List<IndexField>
        {
            //new IndexField { Field = "libid", DisplayName = "online library id" },
            //new IndexField { Field = "keyword", DisplayName = "keywords" }
        };

        public InpxBookProvider(IHostedServices hostedServices)
        {
            _translator = hostedServices.GetTranslator("DotOPDS.BookProvider.Inpx");
        }

        public async IAsyncEnumerable<Book> GetBooksAsync(string library, string[] args)
        {
            var input = args[0];

            if (!File.Exists(input))
            {
                throw new FileNotFoundException("Index file {0} not found.", input);
            }

            using var parser = await InpxParser.Open(input, new Genres(_translator));
            await foreach (var book in parser.GetBooksAsync())
            {
                yield return book;
            }
        }
    }
}
