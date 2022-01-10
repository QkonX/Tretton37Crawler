namespace Tretton37Crawler.Helpers;

public static class UrlExtensions
{
    public static string ConvertDomainToFolderName(string domain)
    {
        return new Uri(domain).Host.Replace('.', '_');
    }
}