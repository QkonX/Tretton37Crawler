using Microsoft.Extensions.Logging;

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

    public async Task<byte[]?> Fetch(Uri requestUri)
    {
        _logger.LogInformation("Downloading is starting: {RelativeUrl}", requestUri.AbsolutePath);

        try
        {
            var responseMessage = await _httpClient.GetAsync(requestUri);

            if (!responseMessage.IsSuccessStatusCode)
            {
                return null;
            }

            _logger.LogInformation("Successfully downloaded: {RelativeUrl} ({Size} bytes)",
                requestUri.AbsolutePath, responseMessage.Content.Headers.ContentLength);

            return await responseMessage.Content.ReadAsByteArrayAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(
                "Downloading failed: {RelativeUrl} (Error message: {Error})",
                requestUri.AbsolutePath, e.Message);

            return null;
        }
    }
}