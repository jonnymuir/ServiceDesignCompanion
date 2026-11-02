using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Extensions;

namespace ServiceDesignCompanion.Site.Content;

/// <summary>Reads a published method page's lists back from the values the seeder stores.</summary>
public static class MethodContent
{
    public static IReadOnlyList<Phase> Phases(this IPublishedContent method) =>
        [.. List(method, SiteContentTypes.Phases, ',').Select(text => Enum.TryParse<Phase>(text, out var phase) ? (Phase?)phase : null).OfType<Phase>()];

    public static IReadOnlyList<int> RelatedPoints(this IPublishedContent method) =>
        [.. List(method, SiteContentTypes.RelatedPoints, ',').Select(text => int.TryParse(text, out var number) ? number : 0).Where(number => number > 0)];

    public static IReadOnlyList<string> Steps(this IPublishedContent method) => List(method, SiteContentTypes.Steps, '\n');

    /// <summary>The published method pages, in the order the seeder gives them.</summary>
    public static IEnumerable<IPublishedContent> Methods(this IPublishedContent anyPage) =>
        anyPage.Root().FirstChild(c => c.ContentType.Alias == SiteContentTypes.MethodsPage)?.Children() ?? [];

    private static string[] List(IPublishedContent content, string alias, char separator) =>
        (content.Value<string>(alias) ?? "").Split(separator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
}
