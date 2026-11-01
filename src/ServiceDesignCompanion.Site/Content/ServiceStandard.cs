namespace ServiceDesignCompanion.Site.Content;

/// <summary>
/// One point of the UK Service Standard. Titles are reused from GOV.UK under the Open Government
/// Licence v3.0; summaries, explainers and questions are written for this app.
/// </summary>
public sealed record StandardPoint(
    int Number,
    string Title,
    string GuidanceSlug,
    string Summary = "",
    string ExplainerMarkup = "",
    IReadOnlyList<string>? Questions = null)
{
    public string GuidanceUrl => $"https://www.gov.uk/service-manual/service-standard/{GuidanceSlug}";
}

public static class ServiceStandard
{
    public const string SourceUrl = "https://www.gov.uk/service-manual/service-standard";

    public static readonly IReadOnlyList<StandardPoint> Points =
    [
        new(1, "Understand users and their needs", "point-1-understand-user-needs",
            Summary: "Find out who your users are, what they are trying to get done and the problem behind it, before you settle on a solution.",
            ExplainerMarkup: """
                <p>Start from the problem people have, not from a service someone has already imagined. Research the whole of what people are trying to do, including the parts that happen outside your service, and test your assumptions early, while they are still cheap to change.</p>
                <p>In practice that means regular research with the people who will use the service, including those who find things hardest; quick, throwaway prototypes to test ideas; and making use of the data you already have, such as analytics and contact centre records.</p>
                """,
            Questions:
            [
                "Who have you researched with, and who is still missing?",
                "Which of your user needs are backed by evidence, and which are still assumptions?",
                "What have you learned that surprised you or changed your plans?",
                "How will you keep learning once the service is live?",
            ]),
        new(2, "Solve a whole problem for users", "point-2-solve-a-whole-problem"),
        new(3, "Provide a joined up experience across all channels", "point-3-join-up-across-channels"),
        new(4, "Make the service simple to use", "point-4-make-the-service-simple-to-use"),
        new(5, "Make sure everyone can use the service", "point-5-make-sure-everyone-can-use-the-service"),
        new(6, "Have a multidisciplinary team", "point-6-have-a-multidisciplinary-team"),
        new(7, "Use agile ways of working", "point-7-use-agile-ways-of-working"),
        new(8, "Iterate and improve frequently", "point-8-iterate-and-improve-frequently"),
        new(9, "Create a secure service which protects users' privacy", "point-9-create-a-secure-service"),
        new(10, "Define what success looks like and publish performance data", "point-10-define-success-publish-performance-data"),
        new(11, "Choose the right tools and technology", "point-11-choose-the-right-tools-and-technology"),
        new(12, "Make new source code open", "point-12-make-new-source-code-open"),
        new(13, "Use and contribute to open standards, common components and patterns", "point-13-use-common-standards-components-patterns"),
        new(14, "Operate a reliable service", "point-14-operate-a-reliable-service"),
    ];
}
