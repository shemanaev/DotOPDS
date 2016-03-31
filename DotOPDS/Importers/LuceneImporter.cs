using DotOPDS.Models;
using Lucene.Net.Analysis.Ru;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Store;
using System;
using Version = Lucene.Net.Util.Version;

namespace DotOPDS.Importers
{
    class LuceneImporter : IBookImporter
    {
        private IndexWriter writer;
        private RussianAnalyzer analyzer;
        private SimpleFSDirectory directory;

        public void Insert(Book book)
        {
            var document = MapBook(book);
            writer.AddDocument(document);
        }

        public void Replace(Book book)
        {
            writer.PrepareCommit();
            try
            {
                var document = MapBook(book);
                writer.DeleteDocuments(new Term("Guid", book.Id.ToString()));
                writer.AddDocument(document);
            }
            catch (Exception)
            {
                writer.Rollback();
            }
            writer.Commit();
        }

        public void Dispose()
        {
            if (writer != null) writer.Dispose();
            if (analyzer != null) analyzer.Dispose();
            if (directory != null) directory.Dispose();
        }

        public void Open(string connection)
        {
            analyzer = new RussianAnalyzer(Version.LUCENE_30);
            directory = new SimpleFSDirectory(new System.IO.DirectoryInfo(connection));
            writer = new IndexWriter(directory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED);
        }

        private Document MapBook(Book book)
        {
            var document = new Document();
            document.Add(new Field("Guid", (book.Id != null ? book.Id : Guid.NewGuid()).ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
            document.Add(new Field("LibraryId", book.LibraryId.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
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
            document.Add(new Field("Annotation", book.Annotation ?? "", Field.Store.YES, Field.Index.NO));
            if (book.Cover != null)
            {
                if (book.Cover.Has != null)
                {
                    document.Add(new Field("Cover.Has", book.Cover.Has.ToString(), Field.Store.YES, Field.Index.NO));
                    if (book.Cover.Has == true)
                    {
                        document.Add(new Field("Cover.Type", book.Cover.ContentType, Field.Store.YES, Field.Index.NO));
                        document.Add(new Field("Cover.Data", book.Cover.Data, 0, book.Cover.Data.Length, Field.Store.YES));
                    }
                }
            }
            

            foreach (var author in book.Authors)
            {
                var fullName = author.GetScreenName();
                var fullNameStore = string.Format("{0},{1},{2}", author.FirstName, author.MiddleName, author.LastName);
                var searchName = author.LastName ?? author.FirstName ?? author.MiddleName ?? "";
                document.Add(new Field("Author", fullName, Field.Store.NO, Field.Index.ANALYZED));
                document.Add(new Field("Author.Exact", fullName, Field.Store.NO, Field.Index.NOT_ANALYZED));
                document.Add(new Field("Author.FullName", fullNameStore, Field.Store.YES, Field.Index.NO));
                document.Add(new Field("Author.SearchName", searchName, Field.Store.NO, Field.Index.NOT_ANALYZED));
            }

            foreach (var genre in book.Genres)
            {
                document.Add(new Field("Genre", genre, Field.Store.YES, Field.Index.NOT_ANALYZED));
            }

            return document;
        }
    }
}
