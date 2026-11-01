using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Strings;

namespace ServiceDesignCompanion.Site.Content;

/// <summary>
/// Creates the site's document types and templates when they are missing. The structure is
/// Home, then The Service Standard, then one page per point.
/// </summary>
public sealed class SiteContentTypes(
    IContentTypeService contentTypeService,
    IDataTypeService dataTypeService,
    ITemplateService templateService,
    IShortStringHelper shortStringHelper)
{
    public const string Home = "home";
    public const string ServiceStandardPage = "serviceStandard";
    public const string StandardPointPage = "standardPoint";

    public const string Intro = "intro";
    public const string PointNumber = "pointNumber";
    public const string Summary = "summary";
    public const string Explainer = "explainer";
    public const string Questions = "questions";
    public const string GuidanceUrl = "guidanceUrl";

    public async Task EnsureAsync()
    {
        var point = await EnsureTypeAsync(StandardPointPage, "Standard point", "icon-checkbox", allowedAsRoot: false,
            (Constants.DataTypes.Guids.NumericGuid, PointNumber, "Point number"),
            (Constants.DataTypes.Guids.TextareaGuid, Summary, "Summary"),
            (Constants.DataTypes.Guids.RichtextEditorGuid, Explainer, "Explainer"),
            (Constants.DataTypes.Guids.TextareaGuid, Questions, "Questions to think about (one per line)"),
            (Constants.DataTypes.Guids.TextstringGuid, GuidanceUrl, "GOV.UK guidance link"));
        var standard = await EnsureTypeAsync(ServiceStandardPage, "The Service Standard", "icon-list", allowedAsRoot: false,
            (Constants.DataTypes.Guids.TextareaGuid, Intro, "Introduction"));
        var home = await EnsureTypeAsync(Home, "Home", "icon-home", allowedAsRoot: true,
            (Constants.DataTypes.Guids.TextareaGuid, Intro, "Introduction"));

        Allow(home, standard);
        Allow(standard, point);
    }

    private async Task<IContentType> EnsureTypeAsync(
        string alias, string name, string icon, bool allowedAsRoot, params (Guid DataType, string Alias, string Name)[] properties)
    {
        var contentType = contentTypeService.Get(alias);
        if (contentType is null)
        {
            contentType = new ContentType(shortStringHelper, -1) { Alias = alias, Name = name, Icon = icon, AllowedAsRoot = allowedAsRoot };
            contentType.AddPropertyGroup("content", "Content");
            var sortOrder = 0;
            foreach (var (dataTypeKey, propertyAlias, propertyName) in properties)
            {
                var dataType = await dataTypeService.GetAsync(dataTypeKey)
                    ?? throw new InvalidOperationException($"Umbraco's built-in data type {dataTypeKey} is missing.");
                contentType.AddPropertyType(new PropertyType(shortStringHelper, dataType, propertyAlias) { Name = propertyName, SortOrder = sortOrder++ }, "content");
            }

            Save(contentType);
        }

        await EnsureTemplateAsync(contentType);
        return contentType;
    }

    private async Task EnsureTemplateAsync(IContentType contentType)
    {
        if (contentType.AllowedTemplates?.Any() == true) return;

        var template = await templateService.GetAsync(contentType.Alias)
            ?? (await templateService.CreateForContentTypeAsync(contentType.Name!, contentType.Alias, contentType.Alias, Constants.Security.SuperUserKey)).Result
            ?? throw new InvalidOperationException($"No template could be created for {contentType.Alias}.");

        contentType.AllowedTemplates = [template];
        contentType.SetDefaultTemplate(template);
        Save(contentType);
    }

    private void Allow(IContentType parent, IContentType child)
    {
        var allowed = (parent.AllowedContentTypes ?? []).ToList();
        if (allowed.Any(sort => sort.Alias == child.Alias)) return;

        allowed.Add(new ContentTypeSort(child.Key, allowed.Count, child.Alias));
        parent.AllowedContentTypes = allowed;
        Save(parent);
    }

    // The synchronous Save is the one that persists a new type with its groups in a single call.
#pragma warning disable CS0618
    private void Save(IContentType contentType) => contentTypeService.Save(contentType);
#pragma warning restore CS0618
}
