namespace Tretton37Crawler.Models;

public class DownloadResult
{
    public int TotalUrlCount { get; set; }
    public long TotalSize { get; set; }

    public static DownloadResult operator +(DownloadResult a, DownloadResult b)
    {
        return new DownloadResult
        {
            TotalUrlCount = a.TotalUrlCount + b.TotalUrlCount, TotalSize = a.TotalSize + b.TotalSize
        };
    }
}