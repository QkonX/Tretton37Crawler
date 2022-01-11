using Tretton37Crawler.Models;

namespace Tretton37Crawler.Services;

public interface ICrawlerService
{
    Task<DownloadResult> Download(string domain);
}