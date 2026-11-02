using System.Text.Json;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Services;

namespace ServiceDesignCompanion.Site.Content;

/// <summary>
/// Makes the published content match this repo on every boot: anything missing is created and
/// anything that has drifted is put back. The repo is the source of truth and the database is
/// disposable, so a fresh install and a long-running one end up with the same site.
/// </summary>
public sealed partial class SiteContentSeeder(
    SiteContentTypes contentTypes,
    IContentService contentService,
    IRuntimeState runtimeState,
    ILogger<SiteContentSeeder> logger)
    : INotificationAsyncHandler<UmbracoApplicationStartedNotification>
{
    private static readonly JsonSerializerOptions CamelCase = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public async Task HandleAsync(UmbracoApplicationStartedNotification notification, CancellationToken cancellationToken)
    {
        if (runtimeState.Level < RuntimeLevel.Run) return;

        await contentTypes.EnsureAsync();

        var home = Reconcile(null, SiteContentTypes.Home, "Pocket Service Design", 0, new Dictionary<string, object?>
        {
            [SiteContentTypes.Intro] = "A pocket companion for service designers and delivery teams: the UK Service Standard in plain words, and the methods that help you meet it.",
        });
        var standard = Reconcile(home, SiteContentTypes.ServiceStandardPage, "The Service Standard", 0, new Dictionary<string, object?>
        {
            [SiteContentTypes.Intro] = "The 14 points public services in the UK are assessed against, each in plain words with questions to help your team reflect.",
        });

        foreach (var point in ServiceStandard.Points)
        {
            Reconcile(standard, SiteContentTypes.StandardPointPage, point.Title, point.Number, new Dictionary<string, object?>
            {
                [SiteContentTypes.PointNumber] = point.Number,
                [SiteContentTypes.Summary] = point.Summary,
                [SiteContentTypes.Explainer] = string.IsNullOrWhiteSpace(point.ExplainerMarkup) ? null : RichText(point.ExplainerMarkup),
                [SiteContentTypes.Questions] = string.Join('\n', point.Questions ?? []),
                [SiteContentTypes.GuidanceUrl] = point.GuidanceUrl,
            });
        }

        Reconcile(home, SiteContentTypes.PhaseFinderPage, "Phase finder", 2, new Dictionary<string, object?>
        {
            [SiteContentTypes.Intro] = "Five questions about your project, and the phase it is probably in.",
        });
        ReconcileMethods(home);
    }

    private void ReconcileMethods(IContent home)
    {
        var methods = Reconcile(home, SiteContentTypes.MethodsPage, "Methods", 1, new Dictionary<string, object?>
        {
            [SiteContentTypes.Intro] = "Service design methods that help a team meet the Standard, with when to use each one and how to start.",
        });

        for (var i = 0; i < MethodCards.All.Count; i++)
        {
            var card = MethodCards.All[i];
            Reconcile(methods, SiteContentTypes.MethodPage, card.Title, i, new Dictionary<string, object?>
            {
                [SiteContentTypes.Category] = card.Category,
                [SiteContentTypes.Summary] = card.Summary,
                [SiteContentTypes.WhenToUse] = card.WhenToUse,
                [SiteContentTypes.Steps] = string.Join('\n', card.Steps),
                [SiteContentTypes.Phases] = string.Join(", ", card.Phases),
                [SiteContentTypes.RelatedPoints] = string.Join(", ", card.Points),
            });
        }
    }

    private IContent Reconcile(IContent? parent, string contentTypeAlias, string name, int sortOrder, Dictionary<string, object?> values)
    {
        var content = FindOrCreate(parent, contentTypeAlias, name);
        var changed = content.Id == 0 || !content.Published || content.SortOrder != sortOrder;
        content.SortOrder = sortOrder;
        changed |= ApplyValues(content, values);

        if (changed) Publish(content);
        return content;
    }

    private IContent FindOrCreate(IContent? parent, string contentTypeAlias, string name)
    {
        var siblings = parent is null
            ? contentService.GetRootContent()
            : contentService.GetPagedChildren(parent.Id, 0, int.MaxValue, out _, null, null, null, true);
        return siblings.FirstOrDefault(c => c.ContentType.Alias == contentTypeAlias && c.Name == name)
            ?? contentService.Create(name, parent?.Id ?? Constants.System.Root, contentTypeAlias);
    }

    private static bool ApplyValues(IContent content, Dictionary<string, object?> values)
    {
        var changed = false;
        foreach (var (alias, raw) in values)
        {
            // Umbraco stores an empty value as no value, so compare and save it that way.
            var value = raw is string text && string.IsNullOrEmpty(text) ? null : raw;
            if (Equals(content.GetValue(alias)?.ToString(), value?.ToString())) continue;
            content.SetValue(alias, value);
            changed = true;
        }

        return changed;
    }

    private void Publish(IContent content)
    {
        var name = content.Name ?? content.Key.ToString();
        contentService.Save(content);
        // The synchronous Publish is the one a seeder can call without a backoffice user context.
#pragma warning disable CS0618
        var result = contentService.Publish(content, ["*"], Constants.Security.SuperUserId);
#pragma warning restore CS0618
        if (result.Success) Published(logger, name);
        else PublishFailed(logger, name, result.Result);
    }

    /// <summary>The persisted <c>Umbraco.RichText</c> value: markup plus an empty block set.</summary>
    private static string RichText(string markup) => JsonSerializer.Serialize(new
    {
        markup = markup.Trim(),
        blocks = new { layout = new { }, contentData = Array.Empty<object>(), settingsData = Array.Empty<object>(), expose = Array.Empty<object>() },
    }, CamelCase);

    [LoggerMessage(Level = LogLevel.Information, Message = "Content seeder: published {Name}.")]
    private static partial void Published(ILogger logger, string name);

    [LoggerMessage(Level = LogLevel.Error, Message = "Content seeder: could not publish {Name}: {Result}")]
    private static partial void PublishFailed(ILogger logger, string name, PublishResultType result);
}
