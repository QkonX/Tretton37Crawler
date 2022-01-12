using Microsoft.Extensions.Logging;
using Tretton37Crawler.Helpers;

namespace Tretton37Crawler.Handlers;

public class FileSystemResourceHandler : IResourceHandler
{
    private readonly string _destinationFolderPath;
    private readonly ILogger<FileSystemResourceHandler> _logger;

    public FileSystemResourceHandler(
        ILogger<FileSystemResourceHandler> logger,
        string destinationFolderPath,
        bool cleanupDestinationFolder = false)
    {
        _logger = logger;
        _destinationFolderPath = destinationFolderPath;

        if (cleanupDestinationFolder)
        {
            DirectoryHelper.DeleteDirectoryIfExists(_destinationFolderPath);
        }
    }

    public async Task Process(string relativeUrl, byte[] content)
    {
        if (relativeUrl == "/")
        {
            relativeUrl = "/index.html";
        }

        if (!Path.HasExtension(relativeUrl))
        {
            relativeUrl = Path.ChangeExtension(relativeUrl, ".html");
        }

        var finalPath = Path.Join(_destinationFolderPath,
            DirectoryHelper.ReplaceInvalidDirectoryNameChars(Path.GetDirectoryName(relativeUrl)!),
            FileHelper.ReplaceInvalidFileNameChars(Path.GetFileName(relativeUrl)));

        try
        {
            DirectoryHelper.CreateDirectoryIfNotExists(Path.GetDirectoryName(finalPath)!);

            await FileHelper.Save(finalPath, content);

            _logger.LogInformation("Saved to disk: {Path}", relativeUrl);
        }
        catch (Exception e)
        {
            _logger.LogError("Failed to write to disk: {Path} (Error: {Error})", relativeUrl, e.Message);
        }
    }
}