using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DebenuPDFLibraryLite1114;

namespace DotOPDS.Utils
{




    // This class abstracts the API to pdf manipulation library
    // It assumes the main operations are:
    //   = Read information and meta data
    //   = Modify information and meta data
    // No support is provided for dealing with the actual document in the pdf file
    //

    public class PdfDoc
    {
        //This class abstracts the actual interface to reading/writing/updating pdf files for properties and meta data

        #region Properties
        private PDFLibrary _pdfObj = null;

        private FileInfo _fileInfo = null;
        private string _inputFileName;
        public string InputFileName
        {
            set { _inputFileName = value; }
        }

        private string _outputFileName;
        public string OutputFileName
        {
            set { _outputFileName = value; }
        }

        private int _retVal = 0;
        public int RetVal
        {
            // Result of get/set info etc
            get { return _retVal; }
        }

        private PdfInfo _info = new PdfInfo();
        public PdfInfo Info
        {
            // get or set the properties for document
            // This is the limited non-xml data
            get
            {   // we clean the data to best defaults if missing
                _info.Author = _pdfObj.GetInformation(1);
                if (_info.Author== null ){
                    _info.Author = "";
                }
                _info.Title = _pdfObj.GetInformation(2);
                if (_info.Title==null   )
                {
                    _info.Title = "";
                }
                _info.Subject = _pdfObj.GetInformation(3);
                if (_info.Subject == null)
                {
                    _info.Subject = "";
                }
                _info.Keywords = _pdfObj.GetInformation(4);
                if (_info.Keywords == null)
                {
                    _info.Keywords = "";
                }
                _info.Creator = _pdfObj.GetInformation(5);
                if (_info.Creator == null)
                {
                    _info.Creator = "";
                }
                _info.Producer = _pdfObj.GetInformation(6);
                if (_info.Producer == null)
                {
                    _info.Producer = "";
                }

                try
                {
                    _info.CreationDate = Convert.ToDateTime(_pdfObj.GetInformation(7));
                }
                catch (Exception)
                {
                    _info.CreationDate = DateTime.MinValue;
                }

                try
                {
                    _info.ModifiedDate = Convert.ToDateTime(_pdfObj.GetInformation(8));
                }
                catch (Exception)
                {
                    _info.ModifiedDate = DateTime.MinValue;
                }

                return _info;
            }
            set
            {
                // if no output file set, then use inputfile (overwrite

                if (_outputFileName == null)
                {
                    _outputFileName = _inputFileName;
                }
                _pdfObj.SetInformation(1, Info.Author);
                _pdfObj.SetInformation(2, Info.Title);
                _pdfObj.SetInformation(3, Info.Subject);
                _pdfObj.SetInformation(4, Info.Keywords);
                _retVal = _pdfObj.SaveToFile(_outputFileName);
            }
        }

        private int _pageCount;
        public int PageCount
        {
            get
            {
                if (((_pdfObj != null)))
                {
                    _pageCount = _pdfObj.PageCount();
                }
                else
                {
                    _pageCount = 0;
                }
                return _pageCount;
            }
        }

        private long _fileLength;
        public long FileLength
        {
            get
            {
                _fileInfo = new FileInfo(_inputFileName);
                _fileLength = _fileInfo.Length;
                return _fileLength;
            }
        }

        private Hashtable _meta;
        // TODO Decide to port/fix and use or not
        public Hashtable MetaData
        {
            // get or set the xml-based meta for the file
            // This is the extensible data that is over and above the information area
            get { return _meta; }
            set { _meta = value; }
        }
        #endregion

        /// <summary>
        /// create a pdf object, load into memory from file
        /// </summary>
        /// <param name="file"></param>
        public PdfDoc(string file)
        {
            _inputFileName = file;
            _pdfObj = new PDFLibrary();
            _retVal = _pdfObj.LoadFromFile(_inputFileName, "");
            // We don't handle files with owner/user password
            // TODO do something with error code, log and return it
            int error = _pdfObj.LastErrorCode();
        }
    }

    /// <summary>
    /// The i
    /// </summary>
    public class PdfInfo
    {
        public string Author { get; set; }
        public string Title { get; set; }
        public string Subject { get; set; }
        public string Keywords { get; set; }
        public string Creator { get; set; }
        public string Producer { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public int Pages { get; set; }
        public long Size { get; set; }


    }

    //=======================================================
    //Service provided by Telerik (www.telerik.com)
    //Conversion powered by NRefactory.
    //Twitter: @telerik
    //Facebook: facebook.com/telerik
    //=======================================================

}
