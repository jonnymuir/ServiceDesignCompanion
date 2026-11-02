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
    public async Task AWrittenPoint_ShowsItsSummaryQuestionsAndGuidanceLink()
    {
        var point = ServiceStandard.Points[0];

        var html = await site.PageTextAsync("/the-service-standard/understand-users-and-their-needs/");

        html.Should().Contain("Point 1 of 14");
        html.Should().Contain(point.Summary);
        html.Should().Contain("Questions to think about");
        html.Should().Contain(point.Questions![0]);
        html.Should().Contain($"href=\"{point.GuidanceUrl}\"");
    }

    [Fact]
    public async Task APointNotYetWritten_SaysSoAndLinksToTheGuidance()
    {
        var html = await site.PageTextAsync("/the-service-standard/operate-a-reliable-service/");

        html.Should().Contain("Our plain-words guide to this point is on its way");
        html.Should().Contain($"href=\"{ServiceStandard.Points[13].GuidanceUrl}\"");
        html.Should().NotContain("Questions to think about");
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
