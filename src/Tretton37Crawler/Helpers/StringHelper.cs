namespace Tretton37Crawler.Helpers;

internal static class StringHelper
{
    public static int GetOccurrencesCount(string str, string pattern)
    {
        int count = 0, lastIndex = 0;

        while ((lastIndex = str.IndexOf(pattern, lastIndex, StringComparison.InvariantCulture)) != -1)
        {
            lastIndex++;
            count++;
        }

        return count;
    }
}