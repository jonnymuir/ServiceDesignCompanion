using ServiceDesignCompanion.Site.Content;

namespace ServiceDesignCompanion.Site.Tests;

[Collection(SharedBootedSite.Name)]
public sealed class SitePagesTests(BootedSite site)
{
    private static readonly string[] PublicPages =
    [
        "/",
        "/the-service-standard/",
        "/the-service-standard/understand-users-and-their-needs/",
        "/the-service-standard/operate-a-reliable-service/",
    ];

    [Fact]
    public async Task TheServiceStandardPage_ListsAllFourteenPoints_InOrder()
    {
        var html = await site.PageTextAsync("/the-service-standard/");

        var positions = ServiceStandard.Points.Select(point => html.IndexOf(point.Title, StringComparison.Ordinal)).ToList();
        positions.Should().AllSatisfy(position => position.Should().BePositive());
        positions.Should().BeInAscendingOrder();
    }

    [Fact]
    public async Task EveryPoint_ShowsItsSummaryQuestionsAndGuidanceLink()
    {
        var html = await site.PageTextAsync("/the-service-standard/");
        foreach (var point in ServiceStandard.Points)
        {
            var path = PointPath(html, point.Title);
            var page = await site.PageTextAsync(path);

            page.Should().Contain($"Point {point.Number} of 14", because: point.Title);
            page.Should().Contain(point.Summary, because: point.Title);
            page.Should().Contain("Questions to think about", because: point.Title);
            page.Should().Contain(point.Questions![0], because: point.Title);
            page.Should().Contain($"href=\"{point.GuidanceUrl}\"", because: point.Title);
        }
    }

    // Follows the link on The Service Standard page rather than guessing the URL.
    private static string PointPath(string standardPage, string title)
    {
        var titleAt = standardPage.IndexOf($">{title}</a>", StringComparison.Ordinal);
        titleAt.Should().BePositive(because: title);
        var hrefAt = standardPage.LastIndexOf("href=\"", titleAt, StringComparison.Ordinal) + "href=\"".Length;
        return standardPage[hrefAt..standardPage.IndexOf('"', hrefAt)];
    }

    [Fact]
    public async Task EveryPublicPage_SaysTheAppIsIndependentAndCreditsTheOpenGovernmentLicence()
    {
        foreach (var path in PublicPages)
        {
            var html = await site.PageTextAsync(path);

            html.Should().Contain("Pocket Service Design is an independent app", because: path);
            html.Should().Contain("Open Government Licence v3.0", because: path);
        }
    }

    [Fact]
    public async Task EveryPublicPage_AllowsOnlyThisSiteToSupplyItsContent()
    {
        foreach (var path in PublicPages)
        {
            var response = await site.Browser().GetAsync(path);

            response.Headers.GetValues("Content-Security-Policy").Single().Should().StartWith("default-src 'self'", because: path);
            response.Headers.GetValues("X-Frame-Options").Single().Should().Be("DENY", because: path);
            response.Headers.GetValues("X-Content-Type-Options").Single().Should().Be("nosniff", because: path);
            response.Headers.GetValues("Strict-Transport-Security").Single().Should().Contain("max-age=", because: path);
        }
    }

    [Fact]
    public async Task TheBackoffice_KeepsUmbracosOwnContentPolicy()
    {
        var response = await site.Browser().GetAsync("/umbraco");

        response.Headers.Contains("Content-Security-Policy").Should().BeFalse();
        response.Headers.GetValues("X-Content-Type-Options").Single().Should().Be("nosniff");
    }
}
