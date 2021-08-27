using CommandLine;
using CommandLine.Text;
using DotOPDS.Shared;

namespace DotOPDS.Manage.Commands
{
    [Verb("mv", HelpText = "Move library to another location.")]
    internal class MoveOptions
    {
        [Value(0, MetaName = "id",
            Required = true,
            HelpText = "Library ID.")]
        public string Id { get; set; }

        [Value(1, MetaName = "to",
            Required = true,
            HelpText = "New library location.")]
        public string To { get; set; }

        [Usage]
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example("Move library with id '296cff32-eb42-418a-ba1c-3b5115ec128c' to 'lib/new/location'.", new MoveOptions
                {
                    Id = "296cff32-eb42-418a-ba1c-3b5115ec128c",
                    To = "lib/new/location"
                });
            }
        }
    }

    internal class MoveCommand
    {
        private readonly LibrariesIndex _libraries;

        public MoveCommand(LibrariesIndex libraries)
        {
            _libraries = libraries;
        }

        public async Task<int> Run(MoveOptions opts)
        {
            var id = Guid.Parse(opts.Id);
            var to = opts.To;

            if (!_libraries.Libraries.ContainsKey(id))
            {
                Console.Error.WriteLine("Library {0} not found.", id);
                return 1;
            }

            var lib = _libraries.Libraries[id];
            var from = lib.Path;
            lib.Path = to;
            await _libraries.Save();
            Console.WriteLine("Library moved from {0} to {1}.", from, to);
            return 0;
        }
    }
}
