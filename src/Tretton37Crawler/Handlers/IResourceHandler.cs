namespace Tretton37Crawler.Handlers;

public interface IResourceHandler
{
    Task Process(string relativeUrl, byte[] content);
}