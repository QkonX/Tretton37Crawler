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

    private async Task<DownloadResult> DownloadRecursively(List<string> urls)
    {
        var result = new DownloadResult();
        var fetchingTasks = new List<Task<FetchingResultModel?>>();

        foreach (var url in urls)
        {
            var requestUri = new Uri(new Uri(_domain!), url);
            
            fetchingTasks.Add(_fetchingService.Fetch(requestUri));
            _visitedUrls.TryAdd(requestUri.AbsolutePath, null);
        }

        var fetchingTaskResults = await Task.WhenAll(fetchingTasks);

        var extractedUrls = new HashSet<string>();
        var resourceHandlerTasks = new List<Task>();

        foreach (var fetchingTaskResult in fetchingTaskResults)
        {
            if (fetchingTaskResult is null)
            {
                continue;
            }

            result.TotalUrlCount++;
            result.TotalSize += fetchingTaskResult.Content.LongLength;
            
            resourceHandlerTasks.Add(_resourceHandler.Process(_downloadPath!,
                fetchingTaskResult.Uri, fetchingTaskResult.Content));
            
            foreach (var extractUrl in HtmlHelper.ExtractUrls(_domain!, fetchingTaskResult.Content))
            {
                extractedUrls.Add(extractUrl);
            }
        }

        var newUrls = extractedUrls.Except(_visitedUrls.Keys).ToList();

        if (newUrls.Any())
        {
            result += await DownloadRecursively(newUrls);
        }
        
        await Task.WhenAll(resourceHandlerTasks);

        return result;
    }
}