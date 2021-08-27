using Microsoft.Extensions.Logging;

namespace DotOPDS.Contract;

public interface IHostedServices
{
    ILogger<T> GetLogger<T>();
    ITranslator GetTranslator(string name);
}
