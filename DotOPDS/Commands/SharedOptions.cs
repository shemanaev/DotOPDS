using CommandLine;
using System;
using System.IO;

namespace DotOPDS.Commands
{
    class SharedOptions
    {
        [Option('c', "config",
            HelpText = "Configuration file.")]
        public string Config { get; set; }

        public SharedOptions()
        {
            Config = Path.GetFullPath(Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "DotOPDS/default.conf"));
        }
    }
}
