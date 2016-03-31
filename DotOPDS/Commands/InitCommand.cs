using CommandLine;
using DotOPDS.Utils;
using System;
using System.IO;

namespace DotOPDS.Commands
{
    [Verb("init",
        HelpText = "Create default config file.")]
    class InitOptions : BaseOptions
    {
    }

    class InitCommand : ICommand
    {
        public int Run(BaseOptions options)
        {
            var filename = Util.Normalize(options.Config);
            if (!File.Exists(filename))
            {
                var resource = string.Format("default.{0}.json", Util.IsLinux ? "nix" : "win");
                Resource.SaveToFile(resource, filename);
                Console.WriteLine("Config file created at {0}", filename);
                return 0;
            }
            else
            {
                Console.Error.WriteLine("File {0} already exists!");
                return 1;
            }
        }
    }
}
