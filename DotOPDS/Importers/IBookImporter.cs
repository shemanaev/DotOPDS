using DotOPDS.Models;
using System;
/// <summary>
/// Base class for the Importer/Indexer
/// </summary>
namespace DotOPDS.Importers
{
    interface IBookImporter : IDisposable
    {
        void Open(string storage);
        void Insert(Book book);
    }
}
