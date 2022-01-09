namespace Tretton37Crawler.Services;

public interface IFetchingService
{
    Task<byte[]?> Fetch(Uri requestUri);
}