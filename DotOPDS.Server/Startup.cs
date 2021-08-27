using DotOPDS.Shared;
using DotOPDS.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System.Text.Json.Serialization;

namespace DotOPDS;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        services
            .AddHttpContextAccessor()
            .AddLocalization()
            .AddControllersWithViews(ConfigureMvcOptions)
            .AddJsonOptions(options =>
               options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull);

        SharedServices.ConfigureServices(services, Configuration);

        services.AddScoped<BookParsersPool>();
        services.AddSingleton<ConverterService>();
        services.AddSingleton<FileUtils>();
        services.AddSingleton<MimeHelper>();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.Use(async (context, next) =>
        {
            context.Response.Headers.Add(HeaderNames.Server, "DotOPDS");
            await next.Invoke();
        });

        var supportedCultures = new[] { "en", "ru" };
        var localizationOptions = new RequestLocalizationOptions()
            .SetDefaultCulture(supportedCultures[0])
            .AddSupportedCultures(supportedCultures)
            .AddSupportedUICultures(supportedCultures);
        app.UseRequestLocalization(localizationOptions);

        app.UseDefaultFiles();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthorization();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapFallbackToFile("index.html");
        });
    }

    private void ConfigureMvcOptions(MvcOptions options)
    {
        options.OutputFormatters.Insert(0, new AtomXmlMediaTypeFormatter());
        options.RespectBrowserAcceptHeader = true;
    }
}
