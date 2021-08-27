using CommandLine;
using CommandLine.Text;
using DotOPDS.Shared;

namespace DotOPDS.Manage.Commands
{
    [Verb("rm", HelpText = "Remove library and books from index.")]
    internal class RemoveOptions
    {
        [Value(0, MetaName = "id",
            Required = true,
            HelpText = "Library ID to delete.")]
        public string Lib { get; set; }

        [Usage]
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example("Remove library with ID '296cff32-eb42-418a-ba1c-3b5115ec128c'.", new RemoveOptions
                {
                    Lib = "296cff32-eb42-418a-ba1c-3b5115ec128c"
                });
            }
        }
    }

    internal class RemoveCommand
    {
        private readonly LibrariesIndex _libraries;
        private readonly LuceneIndexStorage _storage;

        public RemoveCommand(LibrariesIndex libraries, LuceneIndexStorage storage)
        {
            _libraries = libraries;
            _storage = storage;
        }

        public async Task<int> Run(RemoveOptions opts)
        {
            var library = Guid.Parse(opts.Lib);
            if (!_libraries.Libraries.ContainsKey(library))
            {
                Console.Error.WriteLine("Library {0} not found.", library);
                return 1;
            }

            var total = _storage.RemoveLibrary(library.ToString());

            _libraries.RemoveLibrary(library);
            await _libraries.Save();
            Console.WriteLine("Library {0} removed ({1} books).", library, total);

            return 0;
        }
    }
}
