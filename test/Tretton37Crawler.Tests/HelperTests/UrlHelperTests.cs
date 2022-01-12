using Tretton37Crawler.Helpers;
using Xunit;

namespace Tretton37Crawler.Tests.HelperTests;

public class UrlHelperTests
{
    [Fact]
    public void ConvertDomainToFolderName_Should_Replace_Dots_To_Underscores()
    {
        // Arrange
        const string domain = "https://foo.bar";

        // Act
        var result = UrlHelpers.ConvertDomainToFolderName(domain);

        // Assert
        Assert.Equal("foo_bar", result);
    }
}