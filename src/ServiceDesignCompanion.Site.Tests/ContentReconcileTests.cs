using Microsoft.Extensions.DependencyInjection;
using ServiceDesignCompanion.Site.Content;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Services;

namespace ServiceDesignCompanion.Site.Tests;

/// <summary>
/// The repo is the source of truth for the site's content: each boot puts back anything that has
/// drifted and leaves everything else alone.
/// </summary>
[Collection(SharedBootedSite.Name)]
public sealed class ContentReconcileTests(BootedSite site)
{
    private const string PointOnePath = "/the-service-standard/understand-users-and-their-needs/";

    [Fact]
    public async Task ANextBoot_WithNothingChanged_PublishesNothing()
    {
        var before = PublishedVersions();

        await BootAgainAsync();

        PublishedVersions().Should().Be(before);
    }

    [Fact]
    public async Task ANextBoot_PutsBackAnEditMadeOutsideTheRepo()
    {
        const string edit = "An edit made in the backoffice that the repo does not know about.";
        using (var scope = site.Services.CreateScope())
        {
            var contentService = scope.ServiceProvider.GetRequiredService<IContentService>();
            var point = PointOne(contentService);
            point.SetValue(SiteContentTypes.Summary, edit);
            contentService.Save(point);
#pragma warning disable CS0618
            contentService.Publish(point, ["*"], Constants.Security.SuperUserId);
#pragma warning restore CS0618
        }

        (await site.PageTextAsync(PointOnePath)).Should().Contain(edit);

        await BootAgainAsync();

        var html = await site.PageTextAsync(PointOnePath);
        html.Should().NotContain(edit);
        html.Should().Contain(ServiceStandard.Points[0].Summary);
    }

    private async Task BootAgainAsync()
    {
        using var scope = site.Services.CreateScope();
        var events = scope.ServiceProvider.GetRequiredService<IEventAggregator>();
        await events.PublishAsync(new UmbracoApplicationStartedNotification(isRestarting: false));
    }

    private int PublishedVersions()
    {
        using var scope = site.Services.CreateScope();
        var contentService = scope.ServiceProvider.GetRequiredService<IContentService>();
        var root = contentService.GetRootContent().Single(c => c.ContentType.Alias == SiteContentTypes.Home);
        return contentService.GetPagedDescendants(root.Id, 0, int.MaxValue, out _)
            .Prepend(root)
            .Sum(content => contentService.GetVersions(content.Id).Count());
    }

    private static IContent PointOne(IContentService contentService)
    {
        var root = contentService.GetRootContent().Single(c => c.ContentType.Alias == SiteContentTypes.Home);
        return contentService.GetPagedDescendants(root.Id, 0, int.MaxValue, out _)
            .Single(c => c.ContentType.Alias == SiteContentTypes.StandardPointPage && c.Name == ServiceStandard.Points[0].Title);
    }
}
