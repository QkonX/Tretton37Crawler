using Tretton37Crawler.Helpers;
using Xunit;

namespace Tretton37Crawler.Tests.HelperTests;

public class StringHelperTests
{
    [Fact]
    public void GetOccurrencesCount_Should_Return_Expected_Result()
    {
        // Arrange
        const string str = "../../foo/bar/";
        const string pattern = "../";

        // Act
        var result = StringHelper.GetOccurrencesCount(str, pattern);

        // Assert
        Assert.Equal(2, result);
    }
}