namespace ServiceDesignCompanion.Site.Content;

public enum Phase
{
    Discovery,
    Alpha,
    Beta,
    Live,
}

/// <summary>
/// One service design method, written for this app. <see cref="Points"/> are the Service Standard
/// points the method most helps a team meet.
/// </summary>
public sealed record MethodCard(
    string Title,
    string Category,
    string Summary,
    string WhenToUse,
    IReadOnlyList<string> Steps,
    IReadOnlyList<Phase> Phases,
    IReadOnlyList<int> Points);

public static partial class MethodCards
{
    private const Phase D = Phase.Discovery, A = Phase.Alpha, B = Phase.Beta, L = Phase.Live;

    public static readonly IReadOnlyList<Phase> AllPhases = Enum.GetValues<Phase>();

    public static readonly IReadOnlyList<MethodCard> All = [.. Research, .. MakeSense, .. DesignAndTest, .. MeasureAndImprove, .. Teamwork];
}
