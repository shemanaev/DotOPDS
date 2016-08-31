using CommandLine;
using CommandLine.Text;
using DotOPDS.Utils;
using Lucene.Net.Analysis.Ru;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using System;
using System.Collections.Generic;
using Version = Lucene.Net.Util.Version;

namespace DotOPDS.Commands
{
    [Verb("rm",
    HelpText = "Remove library and books from index.")]
    class RmOptions : BaseOptions
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
                yield return new Example("Remove library with ID '296cff32-eb42-418a-ba1c-3b5115ec128c'.", new RmOptions
                {
                    Lib = "296cff32-eb42-418a-ba1c-3b5115ec128c"
                });
            }
        }
    }

    class RmCommand : ICommand
    {
        public int Run(BaseOptions options)
        {
            Settings.Load(options.Config);
            var opts = (RmOptions)options;

            var library = Guid.Parse(opts.Lib);
            if (!Settings.Instance.Libraries.ContainsKey(library))
            {
                Console.Error.WriteLine("Library {0} not found.", library);
                return 1;
            }

            using (var analyzer = new RussianAnalyzer(Version.LUCENE_30))
            using (var directory = new SimpleFSDirectory(new System.IO.DirectoryInfo(Util.Normalize(Settings.Instance.Database))))
            using (var writer = new IndexWriter(directory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED))
            {
                var query = new TermQuery(new Term("LibraryId", library.ToString()));

                int total;
                using (var searcher = new IndexSearcher(directory))
                {
                    var docs = searcher.Search(query, 1);
                    total = docs.TotalHits;
                }

                writer.DeleteDocuments(query);
                writer.Optimize(true);

                Settings.Instance.Libraries.Remove(library);
                Settings.Save();
                Console.WriteLine("Library {0} removed ({1} books).", library, total);
            }
            return 0;
        }
    }
}
