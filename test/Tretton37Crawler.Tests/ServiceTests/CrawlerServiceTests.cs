using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Tretton37Crawler.Handlers;
using Tretton37Crawler.Models;
using Tretton37Crawler.Services;
using Xunit;

namespace Tretton37Crawler.Tests.ServiceTests;

public class CrawlerServiceTests
{
    [Fact]
    public async Task Download_Test()
    {
        // Arrange
        const string domain = "https://www.foo.bar";

        var rootContent = "<a href=\"/foo\"></a>".ConvertToBytes();
        var fooPageContent = "<a href=\"/bar\"></a>".ConvertToBytes();

        var fetchingServiceMock = new Mock<IFetchingService>();
        var resourceHandlerMock = new Mock<IResourceHandler>();
        var loggerMock = new Mock<ILogger<CrawlerService>>();

        fetchingServiceMock
            .Setup(x => x.Fetch(domain, "/"))
            .ReturnsAsync(new FetchingResultModel(domain, "/", rootContent));

        fetchingServiceMock
            .Setup(x => x.Fetch(domain, "/foo"))
            .ReturnsAsync(new FetchingResultModel(domain, "/foo", fooPageContent));

        fetchingServiceMock
            .Setup(x => x.Fetch(domain, "/bar"))
            .ReturnsAsync((FetchingResultModel?) null);

        var crawlerService = new CrawlerService(
            fetchingServiceMock.Object,
            resourceHandlerMock.Object,
            loggerMock.Object);

        // Act
        var result = await crawlerService.Download(domain);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.TotalVisitedUrlCount);
        Assert.Equal(rootContent.Length + fooPageContent.Length, result.TotalDownloadedSizeInBytes);
    }
}