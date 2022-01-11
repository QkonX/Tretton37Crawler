using System.Text;

namespace Tretton37Crawler.Tests;

public static class TestHelpers
{
    public static byte[] ConvertToBytes(this string str)
    {
        return Encoding.UTF8.GetBytes(str);
    }
}