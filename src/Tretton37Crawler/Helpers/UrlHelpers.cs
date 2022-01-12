namespace Tretton37Crawler.Helpers;

internal static class UrlHelpers
{
    public static string ConvertDomainToFolderName(string domain)
    {
        return new Uri(domain).Host.Replace('.', '_');
    }
}