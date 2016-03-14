using CommandLine;
using CommandLine.Text;
using System.Collections.Generic;

namespace DotOPDS.Commands
{
    [Verb("import",
        HelpText = "Displays first lines of a file.")]
    class ImportOptions : SharedOptions
    {
        [Value(0, MetaName = "library path",
            Required = true,
            HelpText = "Base path where books located.")]
        public string Library { get; set; }

        [Value(1, MetaName = "input file",
            Required = true,
            HelpText = "Import contents into internal index.")]
        public string Input { get; set; }

        [Usage]
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example("Import inpx file", new ImportOptions
                {
                    Library = "path/to/library/files",
                    Input = "lib1.inpx",
                });
            }
        }
    }
}
