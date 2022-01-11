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
            relativeUrl = "/index.html";
        }

        if (!Path.HasExtension(relativeUrl))
        {
            relativeUrl = Path.ChangeExtension(relativeUrl, ".html");
        }

        var finalPath = Path.Join(destinationFolderPath,
            Path.GetDirectoryName(relativeUrl)!.ReplaceInvalidDirectoryNameChars(),
            Path.GetFileName(relativeUrl).ReplaceInvalidFileNameChars());

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