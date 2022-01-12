namespace Tretton37Crawler.Models;

public class DownloadResult
{
    public int TotalVisitedUrlCount { get; set; }
    public long TotalDownloadedSizeInBytes { get; set; }

    public static DownloadResult operator +(DownloadResult a, DownloadResult b)
    {
        return new DownloadResult
        {
            TotalVisitedUrlCount = a.TotalVisitedUrlCount + b.TotalVisitedUrlCount,
            TotalDownloadedSizeInBytes = a.TotalDownloadedSizeInBytes + b.TotalDownloadedSizeInBytes
        };
    }
}