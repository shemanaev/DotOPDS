using CommandLine;
using DotOPDS.Manage.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace DotOPDS.Manage;

internal class MainService : IHostedService
{
    private readonly MainServiceOptions _options;
    private readonly IServiceProvider _services;
    private readonly IHostApplicationLifetime _appLifetime;

    public MainService(
        IOptions<MainServiceOptions> options,
        IServiceProvider services,
        IHostApplicationLifetime appLifetime)
    {
        _options = options.Value;
        _services = services;
        _appLifetime = appLifetime;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var exitCode = await Parser.Default.ParseArguments<
            ListOptions,
            MoveOptions,
            RemoveOptions,
            ImportOptions
            //FixtureOptions
        >(_options.Args)
        .MapResult(
            async (ListOptions opts) => await _services.GetRequiredService<ListCommand>().Run(opts),
            async (MoveOptions opts) => await _services.GetRequiredService<MoveCommand>().Run(opts),
            async (RemoveOptions opts) => await _services.GetRequiredService<RemoveCommand>().Run(opts),
            async (ImportOptions opts) => await _services.GetRequiredService<ImportCommand>().Run(opts),
            //async (FixtureOptions opts) => await _services.GetRequiredService<FixtureCommand>().Run(opts),
        errs => Task.FromResult(-1));

        Environment.ExitCode = exitCode;
        _appLifetime.StopApplication();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
