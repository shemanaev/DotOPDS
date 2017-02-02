using DotOPDS.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotOPDS.Utils
{
    //public class NewEntryEventArgs : EventArgs
    //{
    //    public Book Book;
    //}
    //public delegate void NewEntryEventHandler(object sender, NewEntryEventArgs e);
    //public delegate void FinishedEventHandler(object sender);

    public class PdfParser : IDisposable
    {
        // private here
        private GetPdfFiles pdfs;

        public event NewEntryEventHandler OnNewEntry;
        public event FinishedEventHandler OnFinished;

        public PdfParser(string sourceDirectory)
        {
            // open directory, store handle
            // What if not exist? I think it is checked earlier in flow
            pdfs = new GetPdfFiles();
            pdfs.rootDir = sourceDirectory;
            pdfs.fileType = "*.pdf";
        }

        /// <summary>
        /// Start parsing
        /// </summary>
        /// <returns></returns>
        public Task Parse()
        {
            return Task.Run((Action)ParseStart);
        }

        private void ParseStart()
        {

            // Do the pdf open, retrieve details here

            List<FileInfo> pdfList;

            pdfs.ListFiles();
            pdfList = pdfs.fileInfos;
            // we now have a list ofr pdf's in that directory tree
            // Since we have FileInfo objects, we can later do things such as 'checked for newer files'

            foreach (FileInfo fi in pdfList)
            {
                PdfDoc pdf = new PdfDoc(fi.FullName);
                PdfInfo info = pdf.Info;
                if (pdf.RetVal != 0)
                {
                    // TODO log message
                    int a = pdf.RetVal;
                }
                string[] name = info.Author.Split(' ');
                Author author = new Author
                {
                    FirstName = SanitizeName(name.Length >= 2 ? name[1] : null),
                    MiddleName = SanitizeName(name.Length >= 3 ? name[2] : null),
                    LastName = SanitizeName(name[0]),
                };
                var args = new Book
                {
                    Id = Guid.Empty,
                    Authors = new[] { author },
                    Genres = new[] { "Other" , "Other"},
                    Title = info.Title,
                    Series = "",
                    SeriesNo = 0,
                    File = fi.FullName,
                    Size = (int)fi.Length,
                    LibId = 21,     // TODO Figure out decent library number?
                    Del = false,
                    Ext = "pdf",
                    Date = info.CreationDate,
                    Language = "English",
                    Keywords = info.Keywords.Split(','),
                    Archive = "",

                };

                OnNewEntry?.Invoke(this, new NewEntryEventArgs { Book = args });

            }

            OnFinished?.Invoke(this);
        }

        private string SanitizeName(string name)
        {
            if (name == null) return null;
            name = name.Trim();
            if (name == "" || name == "--")
                return null;
            return name;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {

            }
        }
    }
}
