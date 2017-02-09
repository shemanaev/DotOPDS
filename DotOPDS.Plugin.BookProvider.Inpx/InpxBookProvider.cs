using DotOPDS.Plugins;
using System;
using System.IO;
using System.Collections.Generic;

namespace DotOPDS.Plugin.BookProvider.Inpx
{
    public class InpxBookProvider : IBookProvider
    {
        public Version Version => new Version(1, 0);
        public string Name => "inpx importer";
        public string Command => "inpx";
        public string Help => @"Arguments:
  inpx file                     inpx file to import.
";

        public IEnumerable<IndexField> IndexFields => new List<IndexField>
        {
            //new IndexField { Field = "libid", DisplayName = "online library id" },
            //new IndexField { Field = "keyword", DisplayName = "keywords" }
        };

        private IPluginHost _host;
        private InpxParser parser;

        public bool Initialize(IPluginHost host)
        {
            _host = host;
            return true;
        }

        public void Run(string library, string[] args, ImportBookCallback import, ImportFinishedCallback finished)
        {
            var input = _host.NormalizePath(args[0]);

            if (!File.Exists(input))
            {
                throw new FileNotFoundException("Index file {0} not found.", input);
            }
            
            parser = new InpxParser(input);
            parser.Genres = new Genres(_host.GetTranslator());
            parser.OnNewEntry += (s, e) =>
            {
                import(e.Book);
            };
            parser.OnFinished += (s) =>
            {
                finished();
            };
            parser.Parse();
        }

        public void Terminate()
        {
        }
    }
}
