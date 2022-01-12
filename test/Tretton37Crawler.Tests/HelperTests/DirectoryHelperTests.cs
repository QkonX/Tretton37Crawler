using Tretton37Crawler.Helpers;
using Xunit;

namespace Tretton37Crawler.Tests.HelperTests;

public class DirectoryHelperTests
{
    [Fact]
    public void ReplaceInvalidDirectoryNameChars_Should_Return_Cleaned_DirectoryName()
    {
        // Arrange
        const string directoryName = "foo\0bar";

        // Act
        var result = DirectoryHelper.ReplaceInvalidDirectoryNameChars(directoryName);

        // Assert
        Assert.Equal("foo_bar", result);
    }
}