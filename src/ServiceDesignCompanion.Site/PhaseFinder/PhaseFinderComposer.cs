using Microsoft.Extensions.DependencyInjection.Extensions;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Notifications;
using Wayfinder.Engine.Abstractions;
using Wayfinder.Models.ServiceDesign;
using Wayfinder.Umbraco;
using Wayfinder.Umbraco.Extensions;

namespace ServiceDesignCompanion.Site.PhaseFinder;

/// <summary>
/// The phase finder is an anonymous Wayfinder journey with one queue and no handover. Each browser
/// gets its own short-lived visitor cookie so people never share a journey, nothing is audited,
/// and an unfinished or finished journey is deleted within two hours (Wayfinder expires an
/// anonymous draft 30 minutes after it was last touched and sweeps hourly).
/// </summary>
[ComposeAfter(typeof(WayfinderUmbracoComposer))]
public sealed class PhaseFinderComposer : IComposer
{
    public const string BlueprintKey = "phase-finder";
    public const string VisitorQueue = "visitor";

    private static readonly ActorProfile Visitor = new()
    {
        VisibleQueues = [VisitorQueue],
        StartableQueues = [VisitorQueue],
        ActionableQueues = [VisitorQueue],
        RestrictToInstanceOwner = true,
    };

    public void Compose(IUmbracoBuilder builder)
    {
        builder.Services.Replace(ServiceDescriptor.Singleton<IAuditLogStore, NoAuditTrail>());
        builder.Services.AddWayfinderUmbraco(options => options.ResolveAccessProfile = (_, _) => Task.FromResult(Visitor));
        builder.Services.AddWayfinderVisitors(visitors =>
        {
            visitors.CookieLifetime = TimeSpan.FromHours(2);
            visitors.StaffLandingPath = "/";
            visitors.ExpiredLinkPath = "/";
        });
        builder.AddNotificationAsyncHandler<UmbracoApplicationStartedNotification, PhaseFinderBlueprintSeeder>();
    }
}
