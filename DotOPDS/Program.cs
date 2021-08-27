using DotOPDS.Manage;
using DotOPDS.Manage.Commands;
using DotOPDS.Shared;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateDefaultBuilder(args);

// Make console use fancy unicode on Windows
System.Console.OutputEncoding = System.Text.Encoding.UTF8;

// Suppress Lifetime logging
builder.ConfigureAppConfiguration((hostContext, config) =>
{
    config.AddInMemoryCollection(new Dictionary<string, string>
    {
        {"Logging:LogLevel:Microsoft.Hosting.Lifetime", "None"}
    });
});

builder.ConfigureServices((hostContext, services) =>
{
    services.Configure<MainServiceOptions>(options =>
    {
        options.Args = args;
    });

    SharedServices.ConfigureServices(services, hostContext.Configuration);

    services.AddScoped<ListCommand>();
    services.AddScoped<MoveCommand>();
    services.AddScoped<RemoveCommand>();
    services.AddScoped<ImportCommand>();
    //services.AddScoped<FixtureCommand>();

    services.AddSingleton<IHostedService, MainService>();
});

var app = builder.Build();
await app.RunAsync();
//var mainService = app.Services.GetRequiredService<MainService>();
//await mainService.StartAsync();
