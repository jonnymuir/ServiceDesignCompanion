using Wayfinder.Engine.Abstractions;

namespace ServiceDesignCompanion.Site.PhaseFinder;

/// <summary>The site keeps no record of who used the phase finder or what they answered.</summary>
public sealed class NoAuditTrail : IAuditLogStore
{
    public void Record(AuditEvent auditEvent)
    {
    }

    public IReadOnlyList<AuditEvent> GetByInstance(string instanceId) => [];

    public IReadOnlyList<AuditEvent> Query(
        string? instanceId = null,
        string? actor = null,
        AuditEventSeverity? minimumSeverity = null,
        DateTimeOffset? fromUtc = null,
        DateTimeOffset? toUtc = null,
        int pageIndex = 0,
        int pageSize = 50) => [];
}
