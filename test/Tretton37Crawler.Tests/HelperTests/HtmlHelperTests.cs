using System.Linq;
using Tretton37Crawler.Helpers;
using Xunit;

namespace Tretton37Crawler.Tests.HelperTests;

public class HtmlHelperTests
{
    [Fact]
    public void ExtractUrls_Should_Return_Matched_Url()
    {
        // Arrange
        const string domain = "domain.com";
        var content = "<html><body><a href=\"/foo\">bar</a></body></html>".ConvertToBytes();

        // Act
        var result = HtmlHelper.ExtractUrls(domain, "/", content).ToList();

        // Assert
        Assert.Single(result);
        Assert.Equal("/foo", result.First());
    }

    [Theory]
    [InlineData("<a href=\"foo\">bar</a>")]
    [InlineData("<a href=\"/foo\">bar</a>")]
    [InlineData("<a href=\"foo#baz\">bar</a>")]
    [InlineData("<a href=\"\\foo\">bar</a>")]
    public void ExtractUrls_Should_Normalize_Url(string link)
    {
        // Arrange
        const string domain = "domain.com";
        var content = link.ConvertToBytes();

        // Act
        var result = HtmlHelper.ExtractUrls(domain, "/", content).ToList();

        // Assert
        Assert.Single(result);
        Assert.Equal("/foo", result.First());
    }

    [Theory]
    [InlineData("<a href=\"#foo\">bar</a>")]
    [InlineData("<a href=\"//foo\">bar</a>")]
    [InlineData("<a href=\"javascript:foo\">bar</a>")]
    [InlineData("<a href=\"mailto:foo\">bar</a>")]
    [InlineData("<a href=\"tel:foo\">bar</a>")]
    public void ExtractUrls_Should_Not_Return_Invalid_Url(string link)
    {
        // Arrange
        const string domain = "domain.com";
        var content = link.ConvertToBytes();

        // Act
        var result = HtmlHelper.ExtractUrls(domain, "/", content).ToList();

        // Assert
        Assert.Empty(result);
    }

    [Theory]
    [InlineData("<a href=\"/foo.css?v=1\">bar</a>", "/foo.css")]
    [InlineData("<a href=\"/foo.js?v=1\">bar</a>", "/foo.js")]
    public void ExtractUrls_Should_Remove_QueryString_From_Url(string link, string expectedUrl)
    {
        // Arrange
        const string domain = "domain.com";
        var content = link.ConvertToBytes();

        // Act
        var result = HtmlHelper.ExtractUrls(domain, "/", content).ToList();

        // Assert
        Assert.Single(result);
        Assert.Equal(expectedUrl, result.First());
    }

    [Theory]
    [InlineData("<a href=\"/foo.html?v=1\">bar</a>", "/foo.html?v=1")]
    [InlineData("<a href=\"/foo.jpg?v=1\">bar</a>", "/foo.jpg?v=1")]
    public void ExtractUrls_Should_Not_Remove_QueryString_From_Url(string link, string expectedUrl)
    {
        // Arrange
        const string domain = "domain.com";
        var content = link.ConvertToBytes();

        // Act
        var result = HtmlHelper.ExtractUrls(domain, "/", content).ToList();

        // Assert
        Assert.Single(result);
        Assert.Equal(expectedUrl, result.First());
    }

    [Fact]
    public void ExtractUrls_Should_Distinct_Urls()
    {
        // Arrange
        const string domain = "domain.com";
        var content = "<a href=\"/foo\">bar</a><a href=\"/foo\">bar</a><a href=\"/foo\">bar</a>".ConvertToBytes();

        // Act
        var result = HtmlHelper.ExtractUrls(domain, "/", content).ToList();

        // Assert
        Assert.Single(result);
        Assert.Equal("/foo", result.First());
    }

    [Fact]
    public void ExtractUrls_Should_AdjustRelativeUrlToCurrentUrl()
    {
        // Arrange
        const string domain = "domain.com";
        var content = "<a href=\"/../../f\">b</a>".ConvertToBytes();

        // Act
        var result = HtmlHelper.ExtractUrls(domain, "/a/b/c/d/e", content).ToList();

        // Assert
        Assert.Single(result);
        Assert.Equal("/a/b/f", result.First());
    }
}