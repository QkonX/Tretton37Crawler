using System.Text;
using System.Text.RegularExpressions;

namespace Tretton37Crawler.Helpers;

internal static class HtmlHelper
{
    public static IEnumerable<string> ExtractUrls(string domain, byte[] content)
    {
        var responseString = Encoding.UTF8.GetString(content);

        const string pattern = @"href=[\'""]?([^\'"" >]+)";

        var result = new HashSet<string>();

        foreach (Match match in Regex.Matches(responseString, pattern))
        {
            var matchGroup = match.Groups[1];

            if (!matchGroup.Success)
            {
                continue;
            }

            var url = matchGroup.Value;

            if (!IsValidUrl(url))
            {
                continue;
            }

            if (!IsInternalUrl(domain, url))
            {
                continue;
            }

            FixStartingDirectorySeparator(ref url);
            RemoveResourceFileCacheQueryString(ref url);

            if (result.Contains(url))
            {
                continue;
            }

            result.Add(url);
        }

        return result;
    }

    private static void FixStartingDirectorySeparator(ref string url)
    {
        if (url.StartsWith('/'))
        {
            return;
        }

        url = "/" + url;
    }

    private static void RemoveResourceFileCacheQueryString(ref string url)
    {
        if (!url.Contains('?'))
        {
            return;
        }

        var path = url.Split('?')[0];
        var extension = Path.GetExtension(path);

        if (extension is ".css" or ".js")
        {
            url = path;
        }
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