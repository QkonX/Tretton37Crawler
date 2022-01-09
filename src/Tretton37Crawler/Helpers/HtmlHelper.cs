using System.Text;
using System.Text.RegularExpressions;

namespace Tretton37Crawler.Helpers;

internal static class HtmlHelper
{
    public static IEnumerable<string> ExtractUrls(byte[] content)
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

            if (result.Contains(url))
            {
                continue;
            }

            if (!IsInternalUrl(url))
            {
                continue;
            }

            if (url.StartsWith('.'))
            {
                url = "/" + url;
            }

            result.Add(url);
        }

        return result;
    }

    private static bool IsInternalUrl(string url)
    {
        return url.StartsWith('/') || url.StartsWith('.');
    }
}