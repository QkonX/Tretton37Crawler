using Microsoft.Extensions.Logging;
using Tretton37Crawler.Models;

namespace Tretton37Crawler.Services;

public class FetchingService : IFetchingService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<FetchingService> _logger;

    public FetchingService(
        HttpClient httpClient,
        ILogger<FetchingService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<FetchingResultModel?> Fetch(string domain, string relativeUrl)
    {
        _logger.LogInformation("Downloading is starting: {RelativeUrl}", relativeUrl);

        try
        {
            var requestUri = new Uri(new Uri(domain), relativeUrl);
            var response = await _httpClient.GetAsync(requestUri);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            _logger.LogInformation("Successfully downloaded: {RelativeUrl} ({Size} bytes)",
                relativeUrl, response.Content.Headers.ContentLength);

            return new FetchingResultModel(relativeUrl, await response.Content.ReadAsByteArrayAsync());
        }
        catch (Exception e)
        {
            _logger.LogError("Downloading failed: {RelativeUrl} (Error message: {Error})", relativeUrl, e.Message);

            return null;
        }
    }
}