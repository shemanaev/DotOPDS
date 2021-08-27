using DotOPDS.BookProvider.Inpx;
using DotOPDS.Contract;
using DotOPDS.Contract.Plugins;
using DotOPDS.FileFormat.Fb2;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace DotOPDS.Shared.Plugins;

public class PluginProvider
{
    private readonly ILogger<PluginProvider> _logger;
    private readonly IHostedServices _hostedServices;

    private readonly Dictionary<string, Type> fileFormatReaders = new(StringComparer.InvariantCultureIgnoreCase);
    private readonly Dictionary<string, Type> bookProviders = new(StringComparer.InvariantCultureIgnoreCase);
    private readonly List<IndexField> _indexFields = new();

    public List<BookProviderInfo> Providers = new();
    public IEnumerable<IndexField> IndexFields => _indexFields;

    public PluginProvider(
        ILogger<PluginProvider> logger,
        IHostedServices hostedServices)
    {
        _logger = logger;
        _hostedServices = hostedServices;

        var fb2 = new Fb2(_hostedServices);
        fileFormatReaders.Add(fb2.Extension, typeof(Fb2));

        var inpx = new InpxBookProvider(_hostedServices);
        bookProviders.Add(inpx.Command, typeof(InpxBookProvider));
        Providers.Add(new BookProviderInfo(inpx.Name, inpx.Version, inpx.Command, inpx.Help));
        _indexFields.AddRange(inpx.IndexFields);
    }

    public IBookProvider? GetBookProvider(string command)
    {
        if (bookProviders.TryGetValue(command, out Type? provider))
            return Activator.CreateInstance(provider, new object[] { _hostedServices }) as IBookProvider;
        return null;
    }

    public IFileFormat? GetFileFormatReader(string ext)
    {
        if (fileFormatReaders.TryGetValue(ext, out Type? reader))
            return Activator.CreateInstance(reader, new object[] { _hostedServices }) as IFileFormat;
        return null;
    }
}
