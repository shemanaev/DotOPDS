using DotOPDS.Contract;
using DotOPDS.Shared.Options;
using DotOPDS.Shared.Plugins;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DotOPDS.Shared;

public static class SharedServices
{
    public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<PresentationOptions>(configuration.GetSection(PresentationOptions.ConfigurationKey));
        services.Configure<IndexStorageOptions>(configuration.GetSection(IndexStorageOptions.ConfigurationKey));

        services.AddScoped<LuceneIndexStorage>();

        services.AddSingleton<PluginProvider>();
        services.AddSingleton<LibrariesIndex>();
        services.AddSingleton<IHostedServices, HostedServices>();
    }
}
