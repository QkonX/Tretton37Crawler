using Microsoft.Extensions.Logging;
using Tretton37Crawler.Handlers;
using Tretton37Crawler.Helpers;

namespace Tretton37Crawler.Services;

public class CrawlerService : ICrawlerService
{
    private readonly IFetchingService _fetchingService;
    private readonly ILogger<CrawlerService> _logger;
    private readonly IResourceHandler _resourceHandler;

    private string? _domain;
    private string? _downloadPath;

    public CrawlerService(
        IFetchingService fetchingService,
        IResourceHandler resourceHandler,
        ILogger<CrawlerService> logger)
    {
        _fetchingService = fetchingService;
        _resourceHandler = resourceHandler;
        _logger = logger;
    }

    public async Task Download(string domain)
    {
        _domain = domain ?? throw new ArgumentNullException(nameof(domain));
        _downloadPath = Path.Combine(Directory.GetCurrentDirectory(), new Uri(domain).Host.Replace('.', '_'));

        _logger.LogInformation("Downloading starting for {Domain}", domain);

        DirectoryHelper.DeleteDirectoryIfExists(_downloadPath);

        await DownloadRecursively(new HashSet<string>(), "/");

        _logger.LogInformation("Downloading has been completed for {Domain}", domain);
    }

    private async Task DownloadRecursively(ISet<string> visitedUrls, string relativeUrl)
    {
        if (visitedUrls.Contains(relativeUrl))
        {
            return;
        }

        var requestUri = new Uri(new Uri(_domain!), relativeUrl);
        var content = await _fetchingService.Fetch(requestUri);

        visitedUrls.Add(relativeUrl);

        if (content is null)
        {
            return;
        }

        await _resourceHandler.Process(_downloadPath!, requestUri, content);

        var extractedUrls = HtmlHelper.ExtractUrls(content);

        foreach (var extractedUrl in extractedUrls)
        {
            await DownloadRecursively(visitedUrls, extractedUrl);
        }
    }
}