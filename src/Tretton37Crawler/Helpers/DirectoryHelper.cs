namespace Tretton37Crawler.Helpers;

internal static class DirectoryHelper
{
    public static void CreateDirectoryIfNotExists(string path)
    {
        Directory.CreateDirectory(path);
    }

    public static void DeleteDirectoryIfExists(string path)
    {
        if (Directory.Exists(path))
        {
            Directory.Delete(path, true);
        }
    }

    public static string ReplaceInvalidDirectoryNameChars(string directoryName, char replacement = '_')
    {
        return string.Join(replacement, directoryName.Split(Path.GetInvalidPathChars()));
    }
}