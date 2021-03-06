using Tretton37Crawler.Models;

namespace Tretton37Crawler.Services;

public interface IFetchingService
{
    Task<FetchingResultModel?> Fetch(string domain, string relativeUrl);
}