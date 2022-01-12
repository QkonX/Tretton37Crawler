using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Tretton37Crawler.Handlers;
using Tretton37Crawler.Helpers;
using Tretton37Crawler.Services;

var hostBuilder = Host.CreateDefaultBuilder(args);

hostBuilder.ConfigureServices((_, services) => {
    services.AddHttpClient<IFetchingService, FetchingService>();

    services.AddTransient<IResourceHandler>(x => {
        var logger = x.GetRequiredService<ILogger<FileSystemResourceHandler>>();
        var settings = x.GetRequiredService<IConfiguration>();

        var downloadPath = Path.Combine(Directory.GetCurrentDirectory(),
            UrlHelpers.ConvertDomainToFolderName(settings["Domain"]));

        return new FileSystemResourceHandler(logger, downloadPath);
    });

    services.AddTransient<ICrawlerService, CrawlerService>();
});

hostBuilder.ConfigureAppConfiguration(builder => {
    builder.SetBasePath(AppContext.BaseDirectory);
    builder.AddJsonFile("appsettings.json");
});

hostBuilder.ConfigureLogging(builder => {
    builder.AddSimpleConsole(options => {
        options.SingleLine = true;
    });
});

var host = hostBuilder.Build();

var crawlerService = host.Services.GetRequiredService<ICrawlerService>();
var settings = host.Services.GetRequiredService<IConfiguration>();

await crawlerService.Download(settings["Domain"]);

await host.RunAsync();