using CommandLine;
using CommandLine.Text;
using System.Collections.Generic;

namespace DotOPDS.Commands
{
    [Verb("fixture",
        HelpText = "Output c# fixture data for tests.")]
    class FixtureOptions : SharedOptions
    {
        [Value(0, MetaName = "input file",
            Required = true,
            HelpText = "Import contents into internal index.")]
        public string Input { get; set; }

        [Usage]
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example("Output fixture for inpx file to stdout", new FixtureOptions
                {
                    Input = "lib1.inpx",
                });
            }
        }
    }
}
