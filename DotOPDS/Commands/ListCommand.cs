using CommandLine;
using DotOPDS.Shared;

namespace DotOPDS.Manage.Commands
{
    [Verb("ls", HelpText = "List all libraries.")]
    internal class ListOptions { }

    internal class ListCommand
    {
        private readonly LibrariesIndex _libraries;

        public ListCommand(LibrariesIndex libraries)
        {
            _libraries = libraries;
        }

        public Task<int> Run(ListOptions options)
        {
            Console.WriteLine("Libraries available:");
            Console.WriteLine("\tID\t\t\t\t\tLocation");
            foreach (var lib in _libraries.Libraries)
            {
                Console.WriteLine("\t{0}\t{1}", lib.Key, lib.Value.Path);
            }

            return Task.FromResult(0);
        }
    }
}
