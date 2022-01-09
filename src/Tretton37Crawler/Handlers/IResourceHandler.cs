namespace Tretton37Crawler.Handlers;

public interface IResourceHandler
{
    Task Process(string destinationFolderPath, Uri uri, byte[] content);
}