using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Tretton37Crawler.Helpers;

namespace Tretton37Crawler.Handlers;

public class FileSystemResourceHandler : IResourceHandler
{
    private readonly ILogger<FileSystemResourceHandler> _logger;

    public FileSystemResourceHandler(
        ILogger<FileSystemResourceHandler> logger)
    {
        _logger = logger;
    }

    public async Task Process(string destinationFolderPath, Uri uri, byte[] content)
    {
        var relativeUrl = uri.PathAndQuery;

        if (relativeUrl == "/")
        {
            relativeUrl = "index.html";
        }

        if (!Path.HasExtension(relativeUrl))
        {
            relativeUrl += ".html";
        }

        var relativeFilePath = Path.GetFileName(relativeUrl);
        var relativeDirectoryPath = Path.GetDirectoryName(relativeUrl);

        relativeDirectoryPath = string.Join('_', relativeDirectoryPath!.Split(Path.GetInvalidPathChars()));
        relativeFilePath = string.Join('_', relativeFilePath.Split(Path.GetInvalidFileNameChars()));

        var finalPath = Path.Join(destinationFolderPath, relativeDirectoryPath, relativeFilePath);

        try
        {
            DirectoryHelper.CreateDirectoryIfNotExists(Path.GetDirectoryName(finalPath)!);

            await FileHelper.Save(finalPath, content);

            _logger.LogInformation("Saved to disk: {path}", relativeUrl);
        }
        catch (Exception e)
        {
            _logger.LogError("Failed to write to disk: {path} (Error: {error})", relativeUrl, e.Message);
        }
    }
}