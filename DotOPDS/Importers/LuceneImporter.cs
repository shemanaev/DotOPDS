using DotOPDS.Utils;
using Lucene.Net.Analysis.Ru;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Store;
using System;
using Version = Lucene.Net.Util.Version;

namespace DotOPDS.Importers
{
    public class MetaBook
    {
        public Book Book { get; set; }
        public Guid Id { get; set; }
        public Guid LibraryId { get; set; }
    }

    class LuceneImporter : IBookImporter
    {
        private IndexWriter writer;
        private RussianAnalyzer analyzer;
        private SimpleFSDirectory directory;
        private Guid libId;

        public void Insert(Book book)
        {
            var document = MapBook(book);
            writer.AddDocument(document);
        }

        public void Dispose()
        {
            if (writer != null) writer.Dispose();
            if (analyzer != null) analyzer.Dispose();
            if (directory != null) directory.Dispose();
        }

        public void Open(string connection, Guid id)
        {
            libId = id;

            analyzer = new RussianAnalyzer(Version.LUCENE_30);
            directory = new SimpleFSDirectory(new System.IO.DirectoryInfo(connection));
            writer = new IndexWriter(directory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED);
        }

        private Document MapBook(Book book)
        {
            var document = new Document();
            document.Add(new Field("Guid", Guid.NewGuid().ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
            document.Add(new Field("LibraryId", libId.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
            document.Add(new Field("Title", book.Title, Field.Store.YES, Field.Index.ANALYZED));
            document.Add(new Field("Series", book.Series ?? "", Field.Store.YES, Field.Index.ANALYZED));
            document.Add(new Field("Series.Exact", book.Series ?? "", Field.Store.NO, Field.Index.NOT_ANALYZED));
            document.Add(new Field("SeriesNo", book.SeriesNo.ToString(), Field.Store.YES, Field.Index.NO));
            document.Add(new Field("File", book.File, Field.Store.YES, Field.Index.NO));
            document.Add(new Field("Size", book.Size.ToString(), Field.Store.YES, Field.Index.NO));
            document.Add(new Field("LibId", book.LibId.ToString(), Field.Store.YES, Field.Index.ANALYZED));
            document.Add(new Field("Del", book.Del.ToString(), Field.Store.YES, Field.Index.NO));
            document.Add(new Field("Ext", book.Ext, Field.Store.YES, Field.Index.NO));
            document.Add(new Field("Date", book.Date.ToString(), Field.Store.YES, Field.Index.NO));
            document.Add(new Field("Archive", book.Archive, Field.Store.YES, Field.Index.NO));

            foreach (var author in book.Authors)
            {
                var fullName = string.Format("{0} {1} {2}", author.FirstName, author.MiddleName, author.LastName);
                var fullNameStore = string.Format("{0},{1},{2}", author.FirstName, author.MiddleName, author.LastName);
                var searchName = author.LastName ?? author.FirstName ?? author.MiddleName ?? "";
                document.Add(new Field("Author", fullName, Field.Store.NO, Field.Index.ANALYZED));
                document.Add(new Field("Author.Exact", fullName, Field.Store.NO, Field.Index.NOT_ANALYZED));
                document.Add(new Field("Author.FullName", fullNameStore, Field.Store.YES, Field.Index.NO));
                document.Add(new Field("Author.SearchName", searchName, Field.Store.NO, Field.Index.NOT_ANALYZED));
                //document.Add(new Field("Author.FirstName", author.FirstName ?? "", Field.Store.YES, Field.Index.NOT_ANALYZED));
                //document.Add(new Field("Author.MiddleName", author.MiddleName ?? "", Field.Store.YES, Field.Index.NOT_ANALYZED));
                //document.Add(new Field("Author.LastName", author.LastName ?? "", Field.Store.YES, Field.Index.NOT_ANALYZED));
            }

            foreach (var genre in book.Genres)
            {
                document.Add(new Field("Genre", genre, Field.Store.YES, Field.Index.NOT_ANALYZED));
            }

            return document;
        }
    }
}
