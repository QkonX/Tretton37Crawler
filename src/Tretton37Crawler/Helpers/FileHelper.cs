namespace Tretton37Crawler.Helpers;

public class FileHelper
{
    public static async Task Save(string path, byte[] content)
    {
        await File.WriteAllBytesAsync(path, content);
    }
}