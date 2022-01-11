using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Tretton37Crawler.Handlers;
using Tretton37Crawler.Helpers;
using Tretton37Crawler.Models;

namespace Tretton37Crawler.Services;

public class CrawlerService : ICrawlerService
{
    private readonly IFetchingService _fetchingService;
    private readonly ILogger<CrawlerService> _logger;
    private readonly IResourceHandler _resourceHandler;

    private readonly ConcurrentDictionary<string, object?> _visitedUrls;

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

        _visitedUrls = new ConcurrentDictionary<string, object?>();
    }

    public async Task<DownloadResult> Download(string domain)
    {
        _domain = domain ?? throw new ArgumentNullException(nameof(domain));

        _downloadPath = Path.Combine(
            Directory.GetCurrentDirectory(),
            UrlExtensions.ConvertDomainToFolderName(domain));

        _logger.LogInformation("Downloading is starting for {Domain}", domain);

        DirectoryHelper.DeleteDirectoryIfExists(_downloadPath);

        var downloadResult = await DownloadRecursively(new List<string> { "/" });

        _logger.LogInformation(
            "Downloading has been completed for {Domain} ({TotalUrlCount} pages, {TotalSize} bytes)", 
            domain, downloadResult.TotalUrlCount, downloadResult.TotalSize);

        return downloadResult;
    }

    private async Task<DownloadResult> DownloadRecursively(IEnumerable<string> urls)
    {
        var result = new DownloadResult();
        
        var fetchingTasks = urls.Select(url => _fetchingService.Fetch(new Uri(new Uri(_domain!), url)));
        var fetchingTaskResults = await Task.WhenAll(fetchingTasks);

        var extractedUrls = new List<string>();
        var resourceHandlerTasks = new List<Task>();

        foreach (var fetchingResult in fetchingTaskResults)
        {
            if (fetchingResult is null || _visitedUrls.ContainsKey(fetchingResult.Uri.AbsolutePath))
            {
                continue;
            }

            _visitedUrls.TryAdd(fetchingResult.Uri.AbsolutePath, null);

            resourceHandlerTasks.Add(_resourceHandler.Process(_downloadPath!,
                fetchingResult.Uri, fetchingResult.Content));

            extractedUrls.AddRange(HtmlHelper.ExtractUrls(_domain!, fetchingResult.Content));
            
            result.TotalUrlCount++;
            result.TotalSize += fetchingResult.Content.LongLength;
        }

        var nonVisitedUrls = extractedUrls.Except(_visitedUrls.Keys).ToList();

        if (nonVisitedUrls.Any())
        {
            result += await DownloadRecursively(nonVisitedUrls);
        }
        
        await Task.WhenAll(resourceHandlerTasks);

        return result;
    }
}