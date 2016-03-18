using CommandLine;
using System;
using System.IO;

namespace DotOPDS.Commands
{
    class BaseOptions
    {
        [Option('c', "config",
            HelpText = "Configuration file.")]
        public string Config { get; set; }

        public BaseOptions()
        {
            Config = Path.GetFullPath(Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "DotOPDS/default.json"));
        }
    }
}
