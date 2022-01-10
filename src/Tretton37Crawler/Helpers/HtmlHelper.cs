using System.Text;
using System.Text.RegularExpressions;

namespace Tretton37Crawler.Helpers;

internal static class HtmlHelper
{
    public static IEnumerable<string> ExtractUrls(string domain, byte[] content)
    {
        return Regex.Matches(Encoding.UTF8.GetString(content), @"href=[\'""]?([^\'"" >]+)")
            .Select(x => x.Groups[1])
            .Where(x => x.Success)
            .Select(x => x.Value)
            .Where(IsValidUrl)
            .Where(x => IsInternalUrl(domain, x))
            .Select(NormalizeDirectorySeparators)
            .Select(FixStartingDirectorySeparator)
            .Select(RemoveResourceFileQueryString)
            .Distinct();
    }

    private static string NormalizeDirectorySeparators(string url)
    {
        return url.Replace('\\', '/');
    }

    private static string FixStartingDirectorySeparator(string url)
    {
        return url.StartsWith('/') ? url : "/" + url;
    }

    private static string RemoveRelativeUrl(string url)
    {
        return url.Contains('#') ? url.Split('#')[0] : url;
    }
    
    private static string RemoveResourceFileQueryString(string url)
    {
        if (!url.Contains('?'))
        {
            return url;
        }

        var path = url.Split('?')[0];
        var extension = Path.GetExtension(path);

        return extension is ".css" or ".js" ? path : url;
    }

    private static bool IsValidUrl(string url)
    {
        return
            !string.IsNullOrEmpty(url) &&
            !string.IsNullOrWhiteSpace(url) &&
            !url.StartsWith('#') &&
            !url.StartsWith("//") &&
            !url.StartsWith("javascript") &&
            !url.StartsWith("mailto") &&
            !url.StartsWith("tel");
    }

    private static bool IsInternalUrl(string domain, string url)
    {
        return !url.StartsWith("http") || url.Contains(domain);
    }
}