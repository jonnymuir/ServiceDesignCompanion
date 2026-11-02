using UmbracoPrism.Core.Services;

namespace ServiceDesignCompanion.Site.App;

/// <summary>The app's native tabs: the site's four sections.</summary>
public sealed class SiteMobileTabs : IPrismMobileTabsProvider
{
    public static readonly IReadOnlyList<PrismMobileTab> Tabs =
    [
        new("Home", "/", "home"),
        new("Standard", "/the-service-standard/", "/app/tabs/standard.svg"),
        new("Methods", "/methods/", "/app/tabs/methods.svg"),
        new("Phase finder", "/phase-finder/", "/app/tabs/phase-finder.svg"),
    ];

    public Task<IReadOnlyList<PrismMobileTab>> GetTabsAsync(HttpContext context, CancellationToken cancellationToken) => Task.FromResult(Tabs);
}
