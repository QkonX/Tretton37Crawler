namespace Tretton37Crawler.Helpers;

public static class FileHelper
{
    public static async Task Save(string path, byte[] content)
    {
        await File.WriteAllBytesAsync(path, content);
    }

    public static string ReplaceInvalidFileNameChars(this string fileName, char replacement = '_')
    {
        return string.Join(replacement, fileName.Split(Path.GetInvalidFileNameChars()));
    }
}