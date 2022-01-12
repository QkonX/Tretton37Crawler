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
        
        _logger.LogInformation("Downloading is starting for {Domain}", domain);

        var downloadResult = await DownloadRecursively(new List<string> { "/" });

        _logger.LogInformation(
            "Downloading has been completed for {Domain} ({TotalUrlCount} pages, {TotalSize} bytes)", 
            domain, downloadResult.TotalVisitedUrlCount, downloadResult.TotalDownloadedSizeInBytes);

        return downloadResult;
    }

    private async Task<DownloadResult> DownloadRecursively(IEnumerable<string> urls)
    {
        var result = new DownloadResult();
        
        var fetchingTasks = urls.Select(url => _fetchingService.Fetch(_domain!, url));
        var fetchingTaskResults = await Task.WhenAll(fetchingTasks);

        var extractedUrls = new List<string>();
        var resourceHandlerTasks = new List<Task>();

        foreach (var fetchingResult in fetchingTaskResults)
        {
            if (fetchingResult is null)
            {
                continue;
            }

            if (_visitedUrls.ContainsKey(fetchingResult.RelativeUrl))
            {
                continue;
            }

            _visitedUrls.TryAdd(fetchingResult.RelativeUrl, null);

            resourceHandlerTasks.Add(_resourceHandler.Process(
                fetchingResult.RelativeUrl, fetchingResult.Content));

            extractedUrls.AddRange(HtmlHelper.ExtractUrls(_domain!, 
                fetchingResult.RelativeUrl, fetchingResult.Content));
            
            result.TotalVisitedUrlCount++;
            result.TotalDownloadedSizeInBytes += fetchingResult.Content.LongLength;
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