using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Services;
using Wayfinder.Engine.Abstractions;
using Wayfinder.Models.ServiceDesign;

namespace ServiceDesignCompanion.Site.PhaseFinder;

/// <summary>
/// Makes the stored phase finder blueprint match <c>service-blueprints/phase-finder.yaml</c> on
/// every boot, the same way the content seeder treats the site's pages.
/// </summary>
public sealed partial class PhaseFinderBlueprintSeeder(
    IServiceBlueprintSourceStore store,
    IWebHostEnvironment environment,
    IRuntimeState runtimeState,
    ILogger<PhaseFinderBlueprintSeeder> logger)
    : INotificationAsyncHandler<UmbracoApplicationStartedNotification>
{
    public async Task HandleAsync(UmbracoApplicationStartedNotification notification, CancellationToken cancellationToken)
    {
        if (runtimeState.Level < RuntimeLevel.Run) return;

        var path = Path.Combine(environment.ContentRootPath, "service-blueprints", $"{PhaseFinderComposer.BlueprintKey}.yaml");
        var wanted = ServiceBlueprintYaml.Deserialize(await File.ReadAllTextAsync(path, cancellationToken));
        var stored = await store.LoadAsync(PhaseFinderComposer.BlueprintKey, cancellationToken);
        if (stored is not null && SameDesign(stored, wanted)) return;

        var result = await store.SaveAsync(wanted, stored?.Version ?? 0, cancellationToken);
        if (result.Saved) Saved(logger, PhaseFinderComposer.BlueprintKey);
        else SaveFailed(logger, PhaseFinderComposer.BlueprintKey, result.ToString());
    }

    private static bool SameDesign(ServiceBlueprint stored, ServiceBlueprint wanted) =>
        ServiceBlueprintYaml.Serialize(stored with { Version = 0 }) == ServiceBlueprintYaml.Serialize(wanted with { Version = 0 });

    [LoggerMessage(Level = LogLevel.Information, Message = "Blueprint seeder: saved {Key}.")]
    private static partial void Saved(ILogger logger, string key);

    [LoggerMessage(Level = LogLevel.Error, Message = "Blueprint seeder: could not save {Key}: {Result}")]
    private static partial void SaveFailed(ILogger logger, string key, string result);
}
