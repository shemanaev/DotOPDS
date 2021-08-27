using DotOPDS.Shared.Options;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace DotOPDS.Shared;

public class LibrariesIndex
{
    private readonly IndexStorageOptions _storageOptions;
    private readonly string _fileName;
    private readonly LibrariesIndexFile _index;

    public Dictionary<Guid, LibraryItem> Libraries => _index.Libraries;

    public LibrariesIndex(IOptions<IndexStorageOptions> storageOptions)
    {
        _storageOptions = storageOptions.Value;
        _fileName = Path.Combine(_storageOptions.Path!, "index.json");

        if (File.Exists(_fileName))
        {
            LibrariesIndexFile? index = null;
            var jsonString = File.ReadAllText(_fileName);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            try
            {
                index = JsonSerializer.Deserialize<LibrariesIndexFile>(jsonString, options); // TODO: load async
            }
            catch
            {
            }
            _index = index ?? new LibrariesIndexFile();
        }
        else
        {
            _index = new LibrariesIndexFile();
        }
    }

    public string? GetPathById(Guid id)
    {
        if (_index != null && _index.Libraries.TryGetValue(id, out var library))
        {
            return library.Path;
        }
        return null;
    }

    public Guid GetIdByPath(string path)
    {
        if (_index != null)
        {
            return _index.Libraries.FirstOrDefault(lib => path.Equals(lib.Value.Path, StringComparison.InvariantCultureIgnoreCase)).Key;
        }
        return default;
    }

    public void AddLibrary(Guid id, string path)
    {
        Libraries[id] = new LibraryItem(path);
    }

    public void RemoveLibrary(Guid id)
    {
        Libraries.Remove(id);
    }

    public async Task Save()
    {
        if (_index == null) return;
        if (_index.Version == -1) _index.Version = LuceneIndexStorage.VERSION;
        using FileStream createStream = File.Create(_fileName);
        await JsonSerializer.SerializeAsync(createStream, _index);
        await createStream.DisposeAsync();
    }
}
