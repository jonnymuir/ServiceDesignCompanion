using System.Net;
using ServiceDesignCompanion.Site.App;

namespace ServiceDesignCompanion.Site.Tests;

/// <summary>What the native app gets from the site.</summary>
[Collection(SharedBootedSite.Name)]
public sealed class AppTests(BootedSite site)
{
    private const string AppUserAgent = "Mozilla/5.0 (iPhone) AppleWebKit/605.1.15 Mobile/15E148 PrismMobile PrismNativeTabs PrismSavedPages";

    [Fact]
    public async Task TheAppManifest_ListsTheFourSectionsAsTabs()
    {
        var manifest = await site.PageTextAsync("/umbraco/prism/mobile/manifest");

        SiteMobileTabs.Tabs.Should().AllSatisfy(tab => manifest.Should().Contain(tab.Label).And.Contain(tab.Path));
    }

    [Fact]
    public async Task EveryTab_OpensAPage_AndHasAnIcon()
    {
        using var browser = site.Browser();
        foreach (var tab in SiteMobileTabs.Tabs)
        {
            (await browser.GetAsync(tab.Path)).StatusCode.Should().Be(HttpStatusCode.OK, because: tab.Label);
            if (tab.Icon!.StartsWith('/'))
            {
                (await browser.GetAsync(tab.Icon)).StatusCode.Should().Be(HttpStatusCode.OK, because: tab.Icon);
            }
        }
    }

    [Fact]
    public async Task InTheApp_TheNativeTabBarReplacesTheWebMenu()
    {
        using var app = site.Browser();
        app.DefaultRequestHeaders.UserAgent.ParseAdd(AppUserAgent);

        var inApp = await BootedSite.TextOf(app, "/methods/");
        var onTheWeb = await site.PageTextAsync("/methods/");

        inApp.Should().Contain("Pocket Service Design").And.NotContain("aria-label=\"Main\"");
        onTheWeb.Should().Contain("aria-label=\"Main\"");
    }
}
