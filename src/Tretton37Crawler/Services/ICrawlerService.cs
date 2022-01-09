namespace Tretton37Crawler.Services;

public interface ICrawlerService
{
    Task Download(string domain);
}