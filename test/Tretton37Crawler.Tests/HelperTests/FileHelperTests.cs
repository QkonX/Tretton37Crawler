using Tretton37Crawler.Helpers;
using Xunit;

namespace Tretton37Crawler.Tests.HelperTests;

public class FileHelperTests
{
    [Fact]
    public void ReplaceInvalidFileNameChars_Should_Return_Cleaned_FileName()
    {
        // Arrange
        const string fileName = "foo/bar";

        // Act
        var result = FileHelper.ReplaceInvalidFileNameChars(fileName);

        // Assert
        Assert.Equal("foo_bar", result);
    }
}