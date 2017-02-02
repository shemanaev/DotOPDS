// TODO Add my copyright

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace DotOPDS.Utils
{
/// <summary>
/// Recursively scan a directory compiliing a list of all .pdf's
/// </summary>
   public class GetPdfFiles
    {
        #region Properties
        // class to get list of files from a directory tree
        // add error messages to here
        private string __rootDir;
        public string rootDir
        {
            get { return __rootDir; }
            set { __rootDir = value; }
        }

        private string __fileType;
        public string fileType
        {

            // set the file type to filter by

            set { __fileType = value; }
        }

        private List<FileInfo> __filelist = new List<FileInfo>();
        /// <summary>
        /// Returns the list of files as fileInfo objects
        /// </summary>
        public List<FileInfo> fileInfos
        {
            get { return __filelist; }
        }

        private List<string> __files = new List<string>();
        /// <summary>
        /// Returns the list of full file names as a list of strings
        /// </summary>
        public List<string> files
        {
            get { return __files; }
        }
        #endregion
        public void GetFiles(string rootDir)
        {
            __fileType = "*.pdf";
            // default
            __rootDir = rootDir;
        }

        public void ListFiles()
        {
            // from __rootDir, collect all file names that match __fileType

            recursiveListFiles(__fileType, __rootDir);

            foreach (FileInfo f in __filelist)
            {
                __files.Add(f.FullName);
            }
           // return __files;

        }

        private void recursiveListFiles(string fileFilter, string rootDir)
        {
            DirectoryInfo folderInfo = new DirectoryInfo(rootDir);

            DirectoryInfo[] directories = { };
            FileInfo[] files = { };

            try
            {
                directories = folderInfo.GetDirectories();
            }
            catch (UnauthorizedAccessException UAex)
            {
                // TODO log message
            }
            foreach (DirectoryInfo d in directories)
            {
                recursiveListFiles(fileFilter, d.FullName);
            }

            try
            {
                files = folderInfo.GetFiles(fileFilter);
            }
            catch (UnauthorizedAccessException UAex)
            {
                // TODO log message
            }

            foreach (var info_loopVariable in files)
            {
                FileInfo info = info_loopVariable;
                __filelist.Add(info);
            }
        }

    }
}

//=======================================================
//Service provided by Telerik (www.telerik.com)
//Conversion powered by NRefactory.
//Twitter: @telerik
//Facebook: facebook.com/telerik
//=======================================================

