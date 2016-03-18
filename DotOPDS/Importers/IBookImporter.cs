using DotOPDS.Models;
using System;

namespace DotOPDS.Importers
{
    interface IBookImporter : IDisposable
    {
        void Open(string storage, Guid id);
        void Insert(Book book);
    }
}
