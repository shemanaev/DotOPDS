using DotOPDS.Models;
using System;

namespace DotOPDS.Importers
{
    interface IBookImporter : IDisposable
    {
        void Open(string storage);
        void Insert(Book book);
    }
}
