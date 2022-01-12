using System.Text;
using System.Text.RegularExpressions;

namespace Tretton37Crawler.Helpers;

internal static class HtmlHelper
{
    public static IEnumerable<string> ExtractUrls(string domain, string relativeUrl, byte[] content)
    {
        return Regex.Matches(Encoding.UTF8.GetString(content), @"href=[\'""]?([^\'"" >]+)")
            .Select(x => x.Groups[1])
            .Where(x => x.Success)
            .Select(x => x.Value)
            .Where(IsValidUrl)
            .Where(x => IsInternalUrl(domain, x))
            .Select(NormalizePathSeparators)
            .Select(RemoveHtmlRelativeUrl)
            .Select(RemoveResourceFileQueryString)
            .Select(x => AdjustRelativeUrlToCurrentUrl(relativeUrl, x))
            .Distinct();
    }

    private static string NormalizePathSeparators(string url)
    {
        var stringBuilder = new StringBuilder(url);

        if (!url.StartsWith('/') && !url.StartsWith('\\') && !url.StartsWith("../"))
        {
            stringBuilder.Insert(0, '/');
        }

        if (url.StartsWith("/../"))
        {
            stringBuilder.Remove(0, 1);
        }

        return stringBuilder.Replace('\\', '/').ToString();
    }

    private static string RemoveHtmlRelativeUrl(string url)
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

    private static string AdjustRelativeUrlToCurrentUrl(string relativeUrl, string currentUrl)
    {
        const string pattern = "../";

        if (!currentUrl.StartsWith(pattern))
        {
            return currentUrl;
        }

        var previousPathCount = StringHelper.GetOccurrencesCount(currentUrl, pattern);
        var relativeUrlSegments = relativeUrl.TrimStart('/').Split('/');

        var resultSegments = relativeUrlSegments
            .Take(relativeUrlSegments.Length - previousPathCount - 1)
            .Append(currentUrl[(previousPathCount * pattern.Length)..]);

        return $"/{string.Join('/', resultSegments.ToArray())}";
    }
}