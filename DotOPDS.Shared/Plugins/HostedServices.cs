using DotOPDS.Contract;
using DotOPDS.Shared.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DotOPDS.Shared.Plugins;

public class HostedServices : IHostedServices
{
    private readonly ILoggerFactory _loggerFactory;
    private readonly PresentationOptions _options;

    public HostedServices(
        ILoggerFactory loggerFactory,
        IOptions<PresentationOptions> options)
    {
        _loggerFactory = loggerFactory;
        _options = options.Value;
    }

    public ILogger<T> GetLogger<T>()
    {
        return _loggerFactory.CreateLogger<T>();
    }

    public ITranslator GetTranslator(string name)
    {
        return new Translator(name, _options.DefaultLanguage ?? "en");
    }
}
