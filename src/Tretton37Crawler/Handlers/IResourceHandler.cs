namespace Tretton37Crawler.Handlers;

public interface IResourceHandler
{
    Task Process(string destinationFolderPath, string relativeUrl, byte[] content);
}