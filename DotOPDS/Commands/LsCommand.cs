using System;

namespace DotOPDS.Commands
{
    class LsCommand : ICommand
    {
        public int Run(SharedOptions options)
        {
            Settings.Load(options.Config);

            Console.WriteLine("Libraries available:");
            foreach (var lib in Settings.Instance.Libraries)
            {
                Console.WriteLine("\t{0}", lib.Value);
            }

            return 0;
        }

        public void Dispose()
        {
            // do nothing
        }
    }
}
